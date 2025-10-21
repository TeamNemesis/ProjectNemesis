using System;
using UnityEngine;

/// <summary>
/// 플레이어가 획득하는 액티브형 기술 기본 클래스
/// Use()는 실행 시
/// Use(Transform)은 적중시
/// </summary>
public abstract class ActiveTech
{
    [Header("기술 정보")]
    [SerializeField] protected SkillData _skillData; // 기술의 이름
    public SkillData skillData { get {  return _skillData; } }

    public string TechIdx => _skillData.skillIdx.ToString(); // 기술의 번호를 반환하는 속성
    public string TechDescription => _skillData.skillScript; // 기술의 설명을 반환하는 속성
    // public Sprite TechIcon => skillData.skillImagePath; // 기술의 아이콘 이미지를 반환하는 속성
    public int TechLevel => _skillData.skillLevel; // 기술의 레벨을 반환하는 속성


    public abstract event Action OnTechUsed;  // 기술이 사용될 때 발생하는 이벤트

    /// <summary>
    /// 기술이 추가될 때 호출되는 메서드
    /// </summary>
    public virtual void Activate(SkillManager skillManager, PlayerModel player)
    {
       
        switch (_skillData.skillTag)
        {
            case Constants.SKILL_TAG_ATTACK:
                Debug.Log("일반공격 변경");
                skillManager.SetAttackTech(this);
                break;
            case Constants.SKILL_TAG_GEN:
								Debug.Log("유탄 변경");
								skillManager.SetBombTech(this);
                break;
            case Constants.SKILL_TAG_SP:
								Debug.Log("특수공격 변경");
								skillManager.SetSkillTech(this);
                break;
            case Constants.SKILL_TAG_DASH:
								Debug.Log("대쉬 변경");
								skillManager.SetDashTech(this);
                break;
            default:
                Debug.Log("skill Tag : " + _skillData.skillTag);
                break;
        }

        Debug.Log("스킬교체 Active" + _skillData.skillIdx);
    }

    /// <summary>
    /// 기술이 제거될 때 호출되는 메서드
    /// </summary>
    public virtual void Deactivate(PlayerModel player,bool isSameSkill)
    {
        _skillData.RemoveList(isSameSkill);
        Debug.Log("스킬 교체" + _skillData.skillIdx);
    }

    /// <summary>
    /// 기술이 적중될 때 호출되는 메서드
    /// </summary>  
    public virtual void HitEnemy(Transform transform)
    {
        Debug.Log("스킬 사용 ");
    }

    /// <summary>
    /// 공격 시도 시 호출되는 메서드
    /// </summary>
    public virtual void ActiveTry(PlayerModel player)
    {

    }

    public ActiveTech(SkillData skillData)
    {
        _skillData = skillData;
    }
}
