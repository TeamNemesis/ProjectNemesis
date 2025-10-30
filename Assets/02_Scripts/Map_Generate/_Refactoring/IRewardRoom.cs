using System;
using UnityEngine;

public interface IRewardRoom
{
    // 보상 스폰 시 Room 자체가 Spawn 포인트/등록을 담당하게 할 수 있음
    Transform[] RewardSpawnPoints { get; }
    // 보상 요청 이벤트만 제공 (StageController가 Spawn을 호출)
    event Action<IRoom> OnRewardsRequested;
}