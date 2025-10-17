
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
    /// 범위 증가 스킬을 위한 계수
    /// </summary>
    public static float SKILL_EXTENT = 1f;
    #endregion

    #region tag
    /// <summary>
    /// 플레이어 태그
    /// </summary>
    public const string TAG_PLAYER = "Player";

    public const string TAG_MONSTER = "Monster";


    #endregion

    #region skillIndex
    public const int INDEX_ONE_TWO = 60;
    public const int INDEX_FIVE_ONE = 61;
    public const int INDEX_TWO_THREE = 62;
    public const int INDEX_THREE_FOUR = 63;
    public const int INDEX_FOUR_FIVE = 64;

    #endregion


    #region Drone

    /// <summary>
    ///  드론 공격력
    /// </summary>
    public static int DRONE_ATTACK = 2;


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
    #region Bullet
    /// <summary>
    /// 드론 총알 속도
    /// </summary>
    public static float DRONE_BULLET_SPEED = 7f;

    /// <summary>
    /// 드론 총알 존재 시간
    /// </summary>
    public static float DRONE_BULLET_TIME = 5f;

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
    #endregion

    #region layer

    public const string LAYER_MASK_INTERACTABLE = "Interactable";
    public const string LAYER_MASK_PLAYER = "Player";
    public const string LAYER_MASK_WALL = "Wall";

    #endregion

    #region AnimatorParameters
    public static string ANIPARAM_MOVESPEED = "MoveSpeed";
    public static string ANIPARAM_ONNORMALATTACK = "OnNormalAttack";
    public static string ANIPARAM_ONSPECIALATTACK = "OnSpecialAttack";
    public static string ANIPARAM_ONGRENADEATTACK = "OnGrenadeAttack";
    #endregion

    #region ResourcesPath
    public static string RESOURCES_PATH_PLAYER_WEAPONSET = "ScriptableObjects/Player/PlayerWeaponSets";
    #endregion

    #region Map
    public static string RESOURCES_PATH_ROOM_PREFABS = "Prefabs/Map/Rooms";
    public static string RESOURCES_PATH_DOOR_PREFAB = "Prefabs/Map/Doors/Door";
    #endregion
}
