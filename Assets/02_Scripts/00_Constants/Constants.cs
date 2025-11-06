using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Constants
{
    #region skill
    /// <summary>
    /// 스킬 선택, 업그레이드 최대 버튼 개수
    /// </summary>
    public static int SKILLCNT = 3;

    /// <summary>
    /// 최대 스킬 레벨
    /// </summary>
    public static int MAX_SKILLLV = 5;

    /// <summary>
    /// 콜라보 스킬 출현 조건
    /// </summary>
    public static int COLLABCNT = 3;

    /// <summary>
    /// 콜라보 스킬 출현 확률
    /// </summary>
    public static int COLLABPER = 90;

    /// <summary>
    /// 리스트에 포함되지 않은 인덱스
    /// </summary>
    public static int NOCONTAININDEX = -1;

    /// <summary>
    /// 무한루프 방지 횟수
    /// </summary>
    public static int LOOPCNT = 100;

    /// <summary>
    /// 자동회복 주기
    /// </summary>
    public static float HEAL_SECOND = 3.0f;

    /// <summary>
    /// 자동회복 힐량
    /// </summary>
    public static int HEAL_AMOUNT = 1;

    /// <summary>
    /// 기술 분류를 위한 태그
    /// </summary>
    public const string SKILL_TAG_ATTACK = "기본공격";

    public const string SKILL_TAG_GEN = "유탄";

    public const string SKILL_TAG_SP = "특수공격";

    public const string SKILL_TAG_DASH = "대쉬";

    #endregion

    /// <summary>
    /// 일반적인 폭발물 오브젝트 유지 시간
    /// </summary>
    public static float SKILL_REMAIN = 0.5f;


    #region Skill_One

    /// <summary>
    /// 특수 공격 강화(포자 퍼뜨리기) 독 중첩 주기
    /// </summary>
    public static float SKILL_ONE_SPATTACK_STACKTIME = 1f;


    /// <summary>
    /// 피의 갈증 힐량
    /// </summary>
    public static int SKILL_ONE_SPATTACKHEAL = 2;

    /// <summary>
    /// 피격시 독 지속 시간
    /// </summary>
    public static float SKILL_ONE_HITPOISONSPREAD_TIME = 0.5f;

    #endregion

    #region Skill_Two


    /// <summary>
    /// 폭사 데미지
    /// </summary>
    public const float EXPLOSIONDEATH_DAMAGE = 40f;
    /// <summary>
    /// 폭사 범위
    /// </summary>
    public const float EXPLOSIONDEATH_EXTENT = 3f;

    #endregion

    #region tag
    /// <summary>
    /// 플레이어 태그
    /// </summary>
    public const string TAG_PLAYER = "Player";
    public const string TAG_MONSTER = "Monster";
    public const string TAG_BULLET = "Bullet";
    public const string TAG_ENVIRONMENT = "Environment";
    public const string TAG_ELECTIC = "ElectricMan";
    public const string TAG_GROUND = "Ground";
    public const string TAG_WALL = "Wall";

    #endregion

    #region skillIndex
    public const int INDEX_ONE_TWO = 60;
    public const int INDEX_FIVE_ONE = 61;
    public const int INDEX_TWO_THREE = 62;
    public const int INDEX_THREE_FOUR = 63;
    public const int INDEX_FOUR_FIVE = 64;

    #endregion

    public const float MISSILIE_HEIGHT = 0.5f;

    #region Drone

    /// <summary>
    ///  드론 공격력
    /// </summary>
    public static float DRONE_ATTACK = 2;


    /// <summary>
    /// 드론 z 위치 보정
    /// </summary>
    public static float DRONE_Z_POSITION = 0.3f;

    /// <summary>
    /// 드론 회전 속도
    /// </summary>
    public static float DRONE_ROTATION_SPEED = 5f;

    /// <summary>
    /// 드론 사정거리
    /// </summary>
    public static float DRONE_ATTACKRANGE = 10f;

    /// <summary>
    /// 드론 공격 주기
    /// </summary>
    public static float DRONE_ATTACKDELAY = 1.0f;

    /// <summary>
    /// 드론 콜라이더 탐색 숫자
    /// </summary>
    public static int DRONE_SEARCHNUM = 10;

    #endregion

    #region debuff
    /// <summary>
    /// 디버프 갱신 주기
    /// </summary>
    public static float DEBUFF_TIME = 1f;

    public const string DEBUFF_POISON = "poison";
    public const string DEBUFF_OVERLOAD = "overload";
    public const string DEBUFF_SLOW = "slow";
    public const string DEBUFF_STUN = "stun";
    public const string DEBUFF_CONFUSION = "confusion";
    public const string DEBUFF_BURN = "burn";
    public const string DEBUFF_BINDING = "bind";
    public const string DEBUFF_WEAKEN = "weaken";
    #endregion

    #region layer

    public const string LAYER_MASK_INTERACTABLE = "Interactable";
    public const string LAYER_MASK_PLAYER = "Player";
    public const string LAYER_MASK_WALL = "Wall";
    public const string LAYER_MASK_GROUND = "Ground";
    public const string LAYER_MASK_MONSTER = "Monster";

    #endregion

    #region Map Settings
    public const int ROOMCOSTMULTIPLIER = 10;
    #endregion

    #region AnimatorParameters
    public static string ANIPARAM_MOVESPEED = "MoveSpeed";
    public static string ANIPARAM_ONNORMALATTACK = "OnNormalAttack";
    public static string ANIPARAM_ONSPECIALATTACK = "OnSpecialAttack";
    public static string ANIPARAM_ONSPECIALATTACKEND = "OnSpecialAttackEnd";
    public static string ANIPARAM_ONGRENADEATTACK = "OnGrenadeAttack";
    public static string ANIPARAM_ONDASH = "OnDash";
    public static string ANIPARAM_ONDOOROPEN = "OnDoorOpen";
    #endregion

    #region ResourcesPath
    public static string RESOURCES_PATH_PLAYER_WEAPONSET = "ScriptableObjects/Player/PlayerWeaponSets";
    public static string RESOURCES_PATH_REWARD_DATA_SO = "ScriptableObjects/Rewards";


    public const string RESOURCES_PATH_SKILLTOOLTIP = "SkillData/SkillTooltip/KeywordData";
    public const string RESOURCES_PATH_SKILLTOOLTIPUI = "Prefabs/Skill/Skill_ToolTip";
    public const string RESOURCES_PATH_PLAYERSTATDATA = "SkillData/PlayerStatData";
    public static readonly string FILE_PATH_PLAYERSTAT = Path.Combine(Application.dataPath, "SkillData/PlayerStatData.json");


    #region Map
    public static string RESOURCES_PATH_ROOMDATASO = "ScriptableObjects/Map/Rooms";
    public static string RESOURCES_PATH_DOOR_PREFAB = "Prefabs/Map/Doors/Door";
    public static string RESOURCES_PATH_REWARDS = "Prefabs/Rewards";
    public static string RESOURCES_PATH_SHOPITEMS = "Prefabs/ShopItems";
    #endregion

    #endregion

    #region knockBack
    public const float KNOCKBACK_COOLTIME = 5f;
    public const float KNOCKBACK_POWER = 10f;
		#endregion

		#region localization
		public const string STRING_Korean = "ko";
    public const string PREF_KEY = "LanguageIndex";
    #endregion


    #region Util

    /// <summary>
    /// origin과 가장 가까운 List의 요소 반환
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="origin"></param>
    /// <param name="targetList"></param>
    /// <returns></returns>
    public static T GetNearestObject<T>(Transform origin, List<T> targetList)  where T : Component
    {
        if(targetList == null)
        {
            Debug.LogWarning("리스트가 null입니다.");
            return null;
        }


        
        if(targetList.Count == 0)
        {
            Debug.LogWarning("리스트가 비어있습니다.");
            return null;
        }


        float minDistance = float.MaxValue;
        T nearestObject = null;

        foreach(T target in targetList)
        {
            if(target ==null)
            {
                continue;
            }
            float distance = Vector3.Distance(origin.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestObject = target;
            }
        }

        return nearestObject;

    }


    /// <summary>
    /// GameObject를 담은 List용
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="targetList"></param>
    /// <returns></returns>
    public static GameObject GetNearestObject(Transform origin, List<GameObject> targetList) 
    {
        if (targetList == null)
        {
            Debug.LogWarning("리스트가 null입니다.");
            return null;
        }



        if (targetList.Count == 0)
        {
            Debug.LogWarning("리스트가 비어있습니다.");
            return null;
        }


        float minDistance = float.MaxValue;
        GameObject nearestObject = null;

        foreach (GameObject  target in targetList)
        {
            if (target == null)
            {
                continue;
            }
            float distance = Vector3.Distance(origin.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestObject = target;
            }
        }

        return nearestObject;

    }

    #endregion
}

public enum TechSelectPackType
{
    Company1,
    Company2,
    Company3,
    Company4,
    Company5,
    Count,
}

public enum RoomType
{
    Start,
    Normal,
    Lab,
    Colosseum,
    Shop,
    Boss,
}

public enum NormalRoomType
{
    Credit,
    Heal,
    Chrome,
    TechSelect,
    TechUpgrade,
}

public enum ShopItemType
{
    HealPack,
    TechSelectPack,
    TechUpgradePack,
    MutantPack,
}

public enum RewardType
{
    Credit,
    HealPack,
    Chrome,
    TechSelectPack,
    TechUpgradePack,
    MutantPack,
}

public enum InteractableType
{
    Door,
    Reward,
    ShopItem,
    Weapon,
}

/// <summary>
/// 플레이어 어택타입 enum
/// </summary>
public enum ATTACKTYPE
{
    NONE,
    NORMAL,
    GRENADE,
    SPECIALATTACK,
    DASH,
    COUNT
}
