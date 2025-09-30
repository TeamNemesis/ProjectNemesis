using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전역에서(게임 전체에서) 하나만 존재하도록 보장되는 게임 매니저 클래스
/// 게임에서 하나의 객체만 필요한 매니저 등을 관리한다.
/// </summary>
public class GameManager : MonoBehaviour
{
    // 유일한 GameManager 객체를 가리키는 변수
    static GameManager _instance;

    [SerializeField] HeroData _heroData = new();
    public HeroData HeroData => _heroData;

    [SerializeField] ResourceManager _resourceManager;
    public ResourceManager ResourceManager => _resourceManager;

    [SerializeField] PoolManager _poolManager;
    public PoolManager PoolManager => _poolManager;

    /// <summary>
    /// GameManager의 싱글톤 객체(인스턴스)
    /// 필요 시 씬에 GameObject를 자동으로 새로 생성하고
    /// GameManager 컴포넌트를 추가한다.
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            // 유일한 GameManager 객체가 안 만들져 있으면
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameManager>();
                if (_instance == null)
                {
                    // "GameManager"라는 이름으로
                    // 씬에 새 게임오브젝트 생성
                    GameObject go = new("GameManager");

                    // 만들어진 게임오브젝트에 GameManager 컴포넌트 추가
                    _instance = go.AddComponent<GameManager>();

                    // 씬 전환 시 게임오브젝트 제거 방지
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// 싱글톤 초기화 및 중복 객체(인스턴스) 제거
    /// </summary>
    private void Awake()
    {
        // 유일한 GameManager 객체가 없었으면
        if (_instance == null)
        {
            _instance = this; // 싱글톤 인스턴스 설정
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정
        }

        // 유일한 GameManager 객체가 이미 있으면
        else
        {
            Destroy(gameObject);
            return;
        }

        // ResourceManager 컴포넌트 가져오기
        _resourceManager = GetComponent<ResourceManager>();
        // ResourceManager 컴포넌트가 없으면
        if (_resourceManager == null)
        {
            // ResourceManager 추가
            _resourceManager = gameObject.AddComponent<ResourceManager>();
        }

        // PoolManager 컴포넌트 가져오기
        _poolManager = GetComponent<PoolManager>();
        // PoolManager 컴포넌트가 없으면
        if (_poolManager == null)
        {
            // PoolManager 추가
            _poolManager = gameObject.AddComponent<PoolManager>();
        }
        // ResourceManager 초기화
        _resourceManager.Initialze();
        // PoolManager 초기화
        _poolManager.Initialze(_resourceManager);
    }

    // ----- 임시 세이브/로드 ----- //

    private void Update()
    {
        // 임시로 세이브/로드 테스트
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Save();
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(_heroData);
        Debug.Log(json);
        PlayerPrefs.SetString("HeroData", json);
    }

    public void Load()
    {
        string json = PlayerPrefs.GetString("HeroData", string.Empty);
        _heroData = JsonUtility.FromJson<HeroData>(json);
    }

    // ----- 임시 세이브/로드 ----- //
}