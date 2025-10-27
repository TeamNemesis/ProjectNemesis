using UnityEngine;

public class StartRoom : Room
{
    [SerializeField] PoolableObject[] _weapons;

    public override void Initialize(RoomInfo roomInfo)
    {
        base.Initialize(roomInfo);
        SpawnInteractableWeapons();
        RewardSelectionFinished();
    }

    public override RewardInteractableObject[] SpawnReward()
    {
        // 시작방은 보상이 없다.
        return new RewardInteractableObject[0];
    }

    public void SpawnInteractableWeapons()
    {
        if (_weapons.Length != _rewardSpawnPoints.Length)
        {
            Debug.LogWarning("weapons와 rewardSpawnPoints의 길이가 다릅니다.");
            return;
        }

        for (int i = 0; i < _weapons.Length; i++)
        {
            var weaponObj = GameManager.Instance.PoolManager.GetFromPool(_weapons[i], _rewardSpawnPoints[i].position, Quaternion.identity);
            weaponObj.transform.SetParent(_rewardSpawnPoints[i]);
            _poolableObjectsInRoom.Add(weaponObj);
        }
    }
}