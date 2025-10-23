using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private SkillBtn _skillBtnPrefab;
    [SerializeField]
    private GameObject _listPanel;
    #region 현재 보유 스킬 리스트

    [SerializeField]
    private Image _skillImage;
    [SerializeField]
    private Text _skillScriptText;
    [SerializeField]
    private Text _skillLevelText;

    [SerializeField]
    private Transform _parentContent;

    /// <summary>
    /// 보상 선택 이벤트
    /// </summary>
    public event Action onRewardSelect;

    public void InitializeManager()
    {
        if (_skillBtnPrefab == null)
        {
            _skillBtnPrefab = Resources.Load<SkillBtn>("Prefabs/Skill/SkillBtnPrefab");
        }

        if (_skillChooseBtnPrefab == null)
        {
            _skillChooseBtnPrefab = Resources.Load<SkillBtn>("Prefabs/Skill/SkillChoosePrefab");
        }

    }

    /// <summary>
    /// 현재 보유 기술 목록 리스트 제작
    /// </summary>
    public void MakeCurrentSkillList()
    {

        _listPanel.SetActive(true);
        List<SkillData> list = GameManager.Instance.skillManager.GetChooseSkillList();

        // 보유 기술이 없다면 리턴
        if (list == null)
        {
            return;
        }

        foreach (SkillData skill in list)
        {
            MakeSkillBtn(skill, _parentContent);
        }
    }


    /// <summary>
    /// 리스트 버튼 생성
    /// </summary>
    /// <param name="skillData"></param>
    /// <param name="parentContent"></param>
    public void MakeSkillBtn(SkillData skillData, Transform parentContent)
    {


        SkillBtn skillBtn = ObjectPool.Instance.GetFromPool(_skillBtnPrefab, _skillBtnPrefab.transform.position, _skillBtnPrefab.transform.rotation, parentContent).GetComponent<SkillBtn>();
        skillBtn.SetSkillInfo(skillData);
        skillBtn.GetComponent<Button>().onClick.AddListener(() => OnClick_SkillListBtn(skillBtn));

    }

    public void OnClick_SkillListBtn(SkillBtn skillBtn)
    {
        _skillImage.sprite = skillBtn.skillData.skillImagePath;
        _skillScriptText.text = skillBtn.skillData.skillIdx.ToString() + "\n" + skillBtn.skillData.skillScript;
        _skillLevelText.text = skillBtn.skillData.skillLevel.ToString() + " / " + skillBtn.skillData.skillMaxLevel.ToString();
    }

    /// <summary>
    /// 현재 소지 개수 리스트창 자식 오브젝트 파괴용
    /// </summary>
    public void OnClick_ListExitBtn()
    {
        foreach (Transform child in _parentContent.transform)
        {
            PoolableObject childPool = child.GetComponent<PoolableObject>();
            if (childPool != null)
            {
                SkillBtn skillBtn = childPool.GetComponent<SkillBtn>();
                if (skillBtn != null)
                {
                    skillBtn.ReleaseObject();
                }
                ObjectPool.Instance.ReleaseToPoolByInterface(childPool);
            }
        }
    }
    #endregion


    #region 스킬 선택 
    [SerializeField]
    private GameObject _skillBtnPanel;
    [SerializeField]
    private GameObject _parentPanel;
    [SerializeField]
    private SkillBtn _skillChooseBtnPrefab;

    /// <summary>
    /// 보상창 활성화/비활성화
    /// </summary>
    /// <param name="isActive"></param>
    public void SetActiveSkillBtnPanel(bool isActive)
    {
        // 보상창 활성화 상태
        _skillBtnPanel.SetActive(isActive);

        //보상창이 꺼지면 보상 선택 이벤트 발동
        if(isActive == false)
        {
            onRewardSelect?.Invoke();
        }
    }

    public SkillBtn MakeSkillBtn()
    {
        SkillBtn skillBtn = ObjectPool.Instance.GetFromPool(_skillChooseBtnPrefab, Vector3.zero, _skillChooseBtnPrefab.transform.rotation, _parentPanel.transform).GetComponent<SkillBtn>();
        
        return skillBtn;
    }


    #endregion
    public void DestroyChildObject(Transform parentObject)
    {
        Transform[] children = new Transform[parentObject.childCount];
        for (int i = 0; i < parentObject.childCount; i++)
        {
            children[i] = parentObject.GetChild(i);
        }

        foreach (Transform child in children)
        {
            PoolableObject childPool = child.GetComponent<PoolableObject>();
            if (childPool != null)
            {
                SkillBtn skillBtn = childPool.GetComponent<SkillBtn>();
                if (skillBtn != null)
                {
                    skillBtn.ReleaseObject();
                }
                ObjectPool.Instance.ReleaseToPoolByInterface(childPool);
            }
        }

    }

    public void OnClickListExitBtn(Transform content)
    {
        Transform[] children = new Transform[content.childCount];
        for (int i = 0; i < content.childCount; i++)
        {
            children[i] = content.GetChild(i);
        }
        foreach (Transform child in children)
        {
            PoolableObject childPool = child.GetComponent<PoolableObject>();
            Debug.Log(childPool == null);
            if (childPool != null)
            {
                SkillBtn skillBtn = childPool.GetComponent<SkillBtn>();
                if(skillBtn != null)
                {
                    skillBtn.ReleaseObject();
                }
                ObjectPool.Instance.ReleaseToPoolByInterface(childPool);
            }
        }

    _listPanel.SetActive(false);
    }

}
