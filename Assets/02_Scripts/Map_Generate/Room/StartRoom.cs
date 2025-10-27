using UnityEngine;

public class StartRoom : Room
{
    [SerializeField] PoolableObject[] _weapons;
    [SerializeField] PoolableObject[] _cheatItems;

    public override void Initialize(RoomInfo roomInfo)
    {
        base.Initialize(roomInfo);
        SpawnInteractableWeapons();
        RewardSelectionFinished();
        CheatReward();
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

    void CheatReward()
    {
        for(int i=0;i<_cheatItems.Length;i++)
        {
            var cheatItemObj = GameManager.Instance.PoolManager.GetFromPool(_cheatItems[i], new Vector3(2*i,1f,0), Quaternion.identity);
            _poolableObjectsInRoom.Add(cheatItemObj);
        }

    }
}