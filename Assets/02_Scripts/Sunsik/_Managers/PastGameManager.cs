using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 싱글톤 패턴(Singleton Pattern)
// 프로그램 전체에서 단 하나의 객체만 존재하도록 보장하고,
// 그 객체에 전역적으로(글로벌하게) 접근할 수 있게 해 주는 디자인 패턴

// -> 씬끼리 어떤 데이터를 주고받아야 할 때
// -> 게임 전체에 필요한 기능이 있을 경우(리소스 관리, 사운드 재생, 로컬라이징 등)

/// <summary>
/// 
/// </summary>
public class PastGameManager : MonoBehaviour
{
    public static PastGameManager Instance { get; private set; }

    [SerializeField] string _heroName = "Hero";
    public string HeroName => _heroName;

    private void Awake()
    {
        // "Instance" 변수가 아무것도 가리키지 않는 경우
        if (Instance == null)
        {
            // 자신 객체를 Instance로 설정
            Instance = this;

            // 씬 전환 후에도 게임오브젝트가 파괴되지 않게 설정
            DontDestroyOnLoad(gameObject);
        }
        // "Instance" 변수가 가리키는 객체가 있는 경우
        else
        {
            // 혹시라도 이미 있는 경우 파괴
            Destroy(gameObject);
        }
    }

    public void SetHeroName(string heroName)
    {
        _heroName = heroName;
    }
}
