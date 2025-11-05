using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterHealthUI : PoolableObject, IInitializePoolable, IReleasePoolable
{
    [Header("References")]
    [SerializeField] private MonsterBase monsterBase;
    [SerializeField] private Transform monsterTransform;

    [Header("Normal UI Components")]
    [SerializeField] private GameObject normalHealthUI; // 일반 몬스터 UI 루트
    [SerializeField] private Slider normalHealthSlider;

    [Header("Boss UI Components")]
    [SerializeField] private GameObject bossHealthUI; // 보스 UI 루트
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private TextMeshProUGUI bossNameText; // 보스 이름 (선택사항)
    [SerializeField] private TextMeshProUGUI bossHealthText; // 체력 텍스트 (선택사항)

    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private bool hideWhenFull = false;
    [SerializeField] private bool hideWhenDead = true;

    [Header("Color Settings")]
    [SerializeField] private bool useColorGradient = true;
    [SerializeField] private Color highHealthColor = Color.green;
    [SerializeField] private Color midHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;

    private Camera mainCamera;
    private RectTransform rectTransform;
    private Image normalFillImage;
    private Image bossFillImage;

    // 전역 Canvas
    public static Canvas monsterHealthUIRoot;

    // 현재 사용 중인 UI 타입
    private bool isBoss = false;
    private Slider currentSlider;
    private Image currentFillImage;

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

            CanvasScaler scaler = rootObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            rootObj.AddComponent<GraphicRaycaster>();

            Debug.Log("[MonsterHealthUI] MonsterHealthUIRoot Canvas 생성됨");
        }

        // 자신을 MonsterHealthUIRoot의 자식으로 설정
        transform.SetParent(monsterHealthUIRoot.transform, false);

        // Fill Image 찾기
        if (normalHealthSlider != null && normalFillImage == null)
        {
            normalFillImage = normalHealthSlider.fillRect?.GetComponent<Image>();
        }

        if (bossHealthSlider != null && bossFillImage == null)
        {
            bossFillImage = bossHealthSlider.fillRect?.GetComponent<Image>();
        }

        // 초기에는 둘 다 비활성화
        if (normalHealthUI != null)
            normalHealthUI.SetActive(false);
        if (bossHealthUI != null)
            bossHealthUI.SetActive(false);
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

        // 몬스터가 죽었을 때 처리
        if (hideWhenDead && monsterBase.GetMonsterState() == MonsterBase.MonsterState.Die)
        {
            GameManager.Instance.PoolManager.ReleaseToPoolByInterface(this);
            return;
        }

        // 보스는 화면 상단 고정, 일반 몬스터는 머리 위 따라다니기
        if (!isBoss)
        {
            Vector3 worldPosition = monsterTransform.position + offset;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

            // 카메라 뒤에 있으면 숨김
            if (screenPosition.z < 0)
            {
                if (normalHealthUI.activeSelf)
                    normalHealthUI.SetActive(false);
                return;
            }

            if (!normalHealthUI.activeSelf)
                normalHealthUI.SetActive(true);

            rectTransform.position = screenPosition;
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (monsterBase == null || currentSlider == null)
            return;

        float currentHealth = monsterBase.GetCurrentHealth();
        float maxHealth = monsterBase.GetMaxHealth();
        float healthPercentage = currentHealth / maxHealth;

        currentSlider.value = healthPercentage;

        // 색상 변경
        if (useColorGradient && currentFillImage != null)
        {
            if (healthPercentage > 0.5f)
                currentFillImage.color = highHealthColor;
            else if (healthPercentage > 0.25f)
                currentFillImage.color = midHealthColor;
            else
                currentFillImage.color = lowHealthColor;
        }

        // 보스 체력 텍스트 업데이트
        if (isBoss && bossHealthText != null)
        {
            bossHealthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }

        // 체력이 꽉 찼을 때 숨김 옵션
        if (hideWhenFull && healthPercentage >= 0.99f)
        {
            if (isBoss && bossHealthUI.activeSelf)
                bossHealthUI.SetActive(false);
            else if (!isBoss && normalHealthUI.activeSelf)
                normalHealthUI.SetActive(false);
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

        // 보스인지 확인
        isBoss = (monster.GetMonsterSize() == MonsterSize.BIG);

        // UI 전환
        if (isBoss)
        {
            SetupBossUI();
        }
        else
        {
            SetupNormalUI();
        }

        UpdateHealthBar();
    }

    /// <summary>
    /// 일반 몬스터 UI 설정
    /// </summary>
    private void SetupNormalUI()
    {
        // 보스 UI 비활성화
        if (bossHealthUI != null)
            bossHealthUI.SetActive(false);

        // 일반 UI 활성화
        if (normalHealthUI != null)
            normalHealthUI.SetActive(true);

        // 현재 사용 중인 UI 설정
        currentSlider = normalHealthSlider;
        currentFillImage = normalFillImage;

        Debug.Log("[MonsterHealthUI] 일반 몬스터 모드");
    }

    /// <summary>
    /// 보스 UI 설정
    /// </summary>
    private void SetupBossUI()
    {
        // 일반 UI 비활성화
        if (normalHealthUI != null)
            normalHealthUI.SetActive(false);

        // 보스 UI 활성화
        if (bossHealthUI != null)
            bossHealthUI.SetActive(true);

        // 현재 사용 중인 UI 설정
        currentSlider = bossHealthSlider;
        currentFillImage = bossFillImage;

        // 보스 이름 설정
        if (bossNameText != null)
        {
            bossNameText.text = monsterBase.name.Replace("(Clone)", "").Trim();
        }
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

        gameObject.SetActive(true);

        // 일반 몬스터 초기 위치 설정
        if (!isBoss && monsterTransform != null)
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
        isBoss = false;

        if (normalHealthUI != null)
            normalHealthUI.SetActive(false);
        if (bossHealthUI != null)
            bossHealthUI.SetActive(false);

        // Slider 초기화
        if (normalHealthSlider != null)
            normalHealthSlider.value = 1f;
        if (bossHealthSlider != null)
            bossHealthSlider.value = 1f;

        // 색상 초기화
        if (normalFillImage != null)
            normalFillImage.color = highHealthColor;
        if (bossFillImage != null)
            bossFillImage.color = highHealthColor;

        currentSlider = null;
        currentFillImage = null;

        gameObject.SetActive(false);
    }
    #endregion
}