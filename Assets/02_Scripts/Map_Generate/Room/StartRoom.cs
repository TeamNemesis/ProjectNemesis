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
        //CheatReward();
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

    public override IInteractable[] SpawnReward()
    {
        // 시작 방은 보상을 생성하지 않음
        return System.Array.Empty<IInteractable>();
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