using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터를 소환할 수 있는 방 인터페이스
/// </summary>
public interface IMonsterRoom
{
    RoomInfo RoomInfo { get; }
    List<Transform> MonsterSpawnPoints { get; }
}