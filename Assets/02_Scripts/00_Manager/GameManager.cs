using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance()
    {
        return _instance;
    }

    public void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }


        _skillManger.InitializeSkillManager();
    }


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
