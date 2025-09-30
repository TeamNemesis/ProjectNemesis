using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 객체를 Json 형식으로 변환하기 위한 필수 요소(JsonUtility)
// 1) 클래스가 System.Serializable로 설정되어 있어야 한다.
// 2) Json 직렬화/역직렬화를 할 변수(필드)들은 [SerializeField]이거나 public이어야 한다.

/// <summary>
/// 주인공 캐릭터 정보를 저장하는 데이터 클래스.
/// 이름, 스탯 등
/// </summary>
[Serializable]      // 인스펙터뷰 확인용/ 수정용 X
public class HeroData
{
    [SerializeField] string _heroName = "Hero";  // 주인공 이름
    [SerializeField] int _level = 0;
    [SerializeField] float _exp = 0.0f;
    [SerializeField] int _gold = 5;
    [SerializeField] Vector3 _position = Vector3.zero; // 주인공 위치

    public string HeroName => _heroName;
    public int Level => _level;
    public float Exp => _exp;
    public int Gold => _gold;
    public Vector3 Position => _position;

    /// <summary>
    /// 주인공 캐릭터 이름을 변경하는 함수
    /// </summary>
    /// <param name="heroName">새 이름</param>
    public void SetHeroName(string heroName)
    {
        _heroName = heroName;
    }
}
