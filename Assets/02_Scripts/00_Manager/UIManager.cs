using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region 현재 보유 스킬 리스트
    private SkillBtn _skillBtnPrefab;

    [SerializeField]
    private GameObject _listPanel;

    [SerializeField]
    private Text _skillImageText;
    [SerializeField]
    private Text _skillScriptText;
    [SerializeField]
    private Text _skillLevelText;

    [SerializeField]
    private Transform _parentContent;

    public void InitializeManger()
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
        SkillBtn skillBtn = Instantiate(_skillBtnPrefab, parentContent);
        skillBtn.SetSkillInfo(skillData);
        skillBtn.GetComponent<Button>().onClick.AddListener(() => OnClick_SkillListBtn(skillBtn));

    }

    public void OnClick_SkillListBtn(SkillBtn skillBtn)
    {
        _skillImageText.text = skillBtn.skillData.skillImagePath;
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
            Destroy(child.gameObject);
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

    public void SetActiveSkillBtnPanel(bool isActive)
    {
        _skillBtnPanel.SetActive(isActive);
    }

    public SkillBtn MakeSkillBtn()
    {
        return Instantiate(_skillChooseBtnPrefab, _parentPanel.transform);
    }


    #endregion
    public void DestroyChildObject(Transform childObject)
    {
        foreach (Transform child in childObject.parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void DestroyParentChildObject(Transform parentObject)
    {
        foreach (Transform child in parentObject)
        {
            Destroy(child.gameObject);
        }
    }
}
