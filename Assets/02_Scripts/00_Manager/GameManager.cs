using UnityEngine;

/// <summary>
/// 게임 전체에서 싱글톤으로 접근 가능한 매니저 클래스
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    /// <summary>
    /// 어디에서나 접근 가능한 싱글톤 인스턴스
    /// Instance로 접근시 GameManager가 씬에 없으면 자동으로 생성
    /// </summary>
    //public static GameManager Instance
    //{
    //    // get 프로퍼티
    //    get
    //    {
    //        // 만약 GameManager.Instance로 접근했는데 없다면
    //        if (Instance == null)
    //        {
    //            // 씬에서 GameManager를 찾아보고
    //            _instance = FindAnyObjectByType<GameManager>();
    //            // 그래도 없다면
    //            if (_instance == null)
    //            {
    //                // 게임오브젝트를 GameManager라는 이름으로 새로 만들고
    //                GameObject obj = new GameObject("GameManager");
    //                // GameManager 컴포넌트를 추가 후 _instance에 할당
    //                _instance = obj.AddComponent<GameManager>();
    //                // 씬 전환시 파괴되지 않도록 설정
    //                DontDestroyOnLoad(obj);
    //            }
    //        }
    //        // 문제 없이 찾았거나 생성했으면 _instance 반환
    //        return _instance;
    //    }
    //}

    public static GameManager Instance()
    {
        return _instance;
    }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _resourceManager.Initialize();
        //_skillManger.InitializeSkillManager();
    }

    [SerializeField] ResourceManager _resourceManager;      // 리소스 매니저

    public ResourceManager ResourceManager => _resourceManager;

    /// <summary>
    /// 스킬 매니저
    /// </summary>
    [SerializeField]
    private SkillManager _skillManger;
    
    public SkillManager skillManager { get { return _skillManger; } }

    /// <summary>
    /// 플레이어(Test용)
    /// </summary>
    [SerializeField]
    private PlayerModel _player;
    public PlayerModel player { get { return _player; } }
}
