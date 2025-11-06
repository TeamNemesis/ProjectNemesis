using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SkillBtn _skillBtnPrefab;
    [SerializeField] private GameObject _listPanel;

    [SerializeField] private Image _skillImage;
    [SerializeField] private Text _skillScriptText;
    [SerializeField] private Text _skillValueScriptText;
    [SerializeField] private Text _skillLevelText;
    [SerializeField] private Transform _parentContent;

    [SerializeField] private GameObject _skillBtnPanel;
    [SerializeField] private GameObject _parentPanel;
    [SerializeField] private SkillBtn _skillChooseBtnPrefab;

    public event Action onRewardSelect;
    #region 스킬 리스트
    /// <summary>
    /// 현재 보유 스킬 리스트에서 선택한 버튼 정보
    /// </summary>
    private SkillBtn _currentSelectedSkillBtn;

    /// <summary>
    /// 현재 활성화 되어있는 스킬 버튼 리스트
    /// </summary>
    private List<SkillBtn> _activeChooseButtons = new List<SkillBtn>();

    [SerializeField]
    private SkillTooltip _skillTooltip;
    public SkillTooltip skillTooltip { get { return _skillTooltip; } }



    private void ApplySavedResolution()
    {
        int savedIndex = PlayerPrefs.HasKey(Constants.RESOLUTION_PREF_KEY) ? PlayerPrefs.GetInt(Constants.RESOLUTION_PREF_KEY) : 0;

        switch (savedIndex)
        {
            case 0: // PC, default
#if UNITY_STANDALONE_WIN
                QualitySettings.SetQualityLevel(1);
#elif UNITY_ANDROID
                QualitySettings.SetQualityLevel(0);
#endif
                break;
            case 1: // High
                QualitySettings.SetQualityLevel(2);
                break;
            case 2: // Middle
                QualitySettings.SetQualityLevel(3);
                break;
            case 3: // Low
                QualitySettings.SetQualityLevel(4);
                break;
            default:
                Debug.LogWarning("저장된 해상도 인덱스가 유효하지 않습니다.");
                break;
        }
    }

    private void ApplySavedLanguage()
    {
        if (PlayerPrefs.HasKey(Constants.LOCAL_PREF_KEY))
        {
            int savedIndex = PlayerPrefs.GetInt(Constants.LOCAL_PREF_KEY);

            if (savedIndex >= 0 && savedIndex < LocalizationSettings.AvailableLocales.Locales.Count)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[savedIndex];
            }
        }
        else
        {
            // 기본값 설정 (예: 첫 번째 로케일)
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }
    }

    

    public IEnumerator InitializeManager()
    {

        if (_skillBtnPrefab == null)
            _skillBtnPrefab = Resources.Load<SkillBtn>("Prefabs/Skill/SkillBtnPrefab");

        if (_skillChooseBtnPrefab == null)
            _skillChooseBtnPrefab = Resources.Load<SkillBtn>("Prefabs/Skill/SkillChoosePrefab");

        // Localization 시스템 초기화 완료까지 대기
        yield return LocalizationSettings.InitializationOperation;

        // 초기화 완료 후 언어 설정
        ApplySavedLanguage();

        // 해상도 설정
       // ApplySavedResolution();
    }


    public void MakeCurrentSkillList()
    {
        _listPanel.SetActive(true);
        List<SkillData> list = GameManager.Instance.skillManager.GetChooseSkillList();
        if (list == null) return;

        foreach (SkillData skill in list)
        {
            MakeSkillBtn(skill, _parentContent);
        }
    }

    public void MakeSkillBtn(SkillData skillData, Transform parentContent)
    {
        SkillBtn skillBtn = GameManager.Instance.PoolManager
                .GetFromPool(_skillBtnPrefab, _skillBtnPrefab.transform.position, _skillBtnPrefab.transform.rotation, parentContent)
                .GetComponent<SkillBtn>();

        skillBtn.SetSkillInfo(skillData);
        skillBtn.GetComponent<Button>().onClick.AddListener(() => OnClick_SkillListBtn(skillBtn));
    }

    public void OnClick_SkillListBtn(SkillBtn skillBtn)
    {
        // 선택된 버튼 저장
        _currentSelectedSkillBtn = skillBtn;

        SkillData data = skillBtn.skillData;
        _skillImage.sprite = data.skillImagePath;


        string locale = LocalizationSettings.SelectedLocale.Identifier.Code;
        _skillScriptText.text = $"{data.skillIdx}\n" + (locale == "ko" ? data.skillScript : data.skillScriptEn);
        _skillValueScriptText.text = locale == "ko" ? data.skillValueScript : data.skillValueScriptEn;
        _skillLevelText.text = $"{data.skillLevel} / {data.skillMaxLevel}";
    }
    /// <summary>
    /// 언어 변경 시 UI 갱신
    /// </summary>
    public void RefreshCurrentSkillUI()
    {
        if (_currentSelectedSkillBtn != null)
        {
            OnClick_SkillListBtn(_currentSelectedSkillBtn);
        }
    }

    public void OnClick_ListExitBtn()
    {
        foreach (Transform child in _parentContent)
        {
            PoolableObject childPool = child.GetComponent<PoolableObject>();
            if (childPool != null)
            {
                SkillBtn skillBtn = childPool.GetComponent<SkillBtn>();
                if (skillBtn != null) skillBtn.ReleaseObject();
                GameManager.Instance.PoolManager.ReleaseToPoolByInterface(childPool);
            }
        }
    }
    #endregion

    public void SetActiveSkillBtnPanel(bool isActive)
    {
        // 보상패널 활성화 상태를 이벤트버스에 설정
        EventBus.SetIsRewardSelecting(isActive);
        Debug.Log("현재 보상 선택 상태: " + isActive);
        // 콜로세움이면 마우스 커서 켜주기
        if (EventBus.IsColosseumRoom)
        {
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isActive; // 커서 보이게/숨기기
        }

        _skillBtnPanel.SetActive(isActive);

        if (!isActive)
            onRewardSelect?.Invoke();
    }

    #region skill choose
    public SkillBtn MakeSkillBtn()
    {
        SkillBtn skillBtn = GameManager.Instance.PoolManager
                .GetFromPool(_skillChooseBtnPrefab, Vector3.zero, _skillChooseBtnPrefab.transform.rotation, _parentPanel.transform)
                .GetComponent<SkillBtn>();

        _activeChooseButtons.Add(skillBtn);
        return skillBtn;
    }

    /// <summary>
    /// 버튼 텍스트 갱신
    /// </summary>
    public void RefreshAllChooseButtons()
    {
        foreach (SkillBtn button in _activeChooseButtons)
        {
            button.RefreshLanguage();
        }
    }


    public void DestroyChildObject(Transform parentObject)
    {
        Transform[] children = new Transform[parentObject.childCount];
        for (int i = 0; i < parentObject.childCount; i++)
            children[i] = parentObject.GetChild(i);

        foreach (Transform child in children)
        {
            PoolableObject childPool = child.GetComponent<PoolableObject>();
            if (childPool != null)
            {
                SkillBtn skillBtn = childPool.GetComponent<SkillBtn>();
                if (skillBtn != null) skillBtn.ReleaseObject();
                GameManager.Instance.PoolManager.ReleaseToPoolByInterface(childPool);
            }
        }
    }

    public void OnClickListExitBtn(Transform content)
    {
        Transform[] children = new Transform[content.childCount];
        for (int i = 0; i < content.childCount; i++)
            children[i] = content.GetChild(i);

        foreach (Transform child in children)
        {
            PoolableObject childPool = child.GetComponent<PoolableObject>();
            if (childPool != null)
            {
                SkillBtn skillBtn = childPool.GetComponent<SkillBtn>();
                if (skillBtn != null) skillBtn.ReleaseObject();
                GameManager.Instance.PoolManager.ReleaseToPoolByInterface(childPool);
            }
        }

        _listPanel.SetActive(false);
    }
    #endregion
}
