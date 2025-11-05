using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthUI : PoolableObject, IInitializePoolable, IReleasePoolable
{
    [Header("References")]
    [SerializeField] private MonsterBase monsterBase;
    [SerializeField] private Transform monsterTransform;

    [Header("UI Components")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Slider healthSlider;

    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private bool hideWhenFull = false;
    [SerializeField] private bool hideWhenDead = true;

    [Header("Color Settings")]
    [SerializeField] private bool useColorGradient = true;
    [SerializeField] private Color highHealthColor = Color.green;
    [SerializeField] private Color midHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;

    [SerializeField] private Camera mainCamera;
    private RectTransform rectTransform;
    private Image fillImage;

    // 추가: MonsterHealthUIRoot 전역 Canvas
    public static Canvas monsterHealthUIRoot;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // MonsterHealthUIRoot 존재 확인 및 생성
        if (monsterHealthUIRoot == null)
        {
            GameObject rootObj = new GameObject("MonsterHealthUIRoot");
            monsterHealthUIRoot = rootObj.AddComponent<Canvas>();
            monsterHealthUIRoot.renderMode = RenderMode.ScreenSpaceOverlay;
            monsterHealthUIRoot.sortingOrder = 100;
            rootObj.AddComponent<CanvasScaler>();
            rootObj.AddComponent<GraphicRaycaster>();

            Debug.Log("[MonsterHealthUI] MonsterHealthUIRoot Canvas 생성됨");
        }

        // 자신을 MonsterHealthUIRoot의 자식으로 설정
        transform.SetParent(monsterHealthUIRoot.transform, false);

        // 내부 Canvas는 제거 (자기 자신이 따로 Canvas를 가지면 좌표계 꼬임)
        if (canvas != null)
        {
            Destroy(canvas);
        }

        // UI용 Canvas는 MonsterHealthUIRoot에서 담당하므로 참조만 null로 유지
        canvas = monsterHealthUIRoot;

        // Slider Fill Image 찾기
        if (healthSlider != null && fillImage == null)
        {
            fillImage = healthSlider.fillRect?.GetComponent<Image>();
        }
    }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (monsterBase == null || monsterTransform == null)
            return;

        if (hideWhenDead && monsterBase.GetMonsterState() == MonsterBase.MonsterState.Die)
        {
            GameManager.Instance.PoolManager.ReleaseToPoolByInterface(this);
            return;
        }

        Vector3 worldPosition = monsterTransform.position + offset;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        // 카메라 뒤면 비활성화
        if (screenPosition.z < 0)
        {
            if (healthSlider.gameObject.activeSelf)
                healthSlider.gameObject.SetActive(false);
            return;
        }

        if (!healthSlider.gameObject.activeSelf)
            healthSlider.gameObject.SetActive(true);

        rectTransform.position = screenPosition;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (monsterBase == null || healthSlider == null)
            return;

        float currentHealth = monsterBase.GetCurrentHealth();
        float maxHealth = monsterBase.GetMaxHealth();
        healthSlider.value = currentHealth / maxHealth;

        if (useColorGradient && fillImage != null)
        {
            float pct = healthSlider.value;
            if (pct > 0.5f)
                fillImage.color = highHealthColor;
            else if (pct > 0.25f)
                fillImage.color = midHealthColor;
            else
                fillImage.color = lowHealthColor;
        }

        if (hideWhenFull && healthSlider.value >= 0.99f)
        {
            healthSlider.gameObject.SetActive(false);
        }
        else if (!healthSlider.gameObject.activeSelf)
        {
            healthSlider.gameObject.SetActive(true);
        }
    }

    public void SetMonster(MonsterBase monster)
    {
        monsterBase = monster;
        monsterTransform = monster.transform;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (healthSlider != null && fillImage == null)
        {
            fillImage = healthSlider.fillRect?.GetComponent<Image>();
        }

        UpdateHealthBar();
    }

    public void OnHealthChanged()
    {
        UpdateHealthBar();
    }

    #region IInitializePoolable
    public void Initialize(object data = null)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (data is MonsterBase monster)
        {
            SetMonster(monster);
        }

        if (monsterTransform != null)
        {
            Vector3 worldPosition = monsterTransform.position + offset;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            rectTransform.position = screenPosition;
        }

        UpdateHealthBar();
    }
    #endregion

    #region IReleasePoolable
    public void ReleaseObjectPool()
    {
        monsterBase = null;
        monsterTransform = null;

        if (healthSlider != null)
        {
            healthSlider.value = 1f;
        }

        if (fillImage != null)
        {
            fillImage.color = highHealthColor;
        }

        gameObject.SetActive(false);
    }
    #endregion
}
