using UnityEngine;

public class StartRoom : Room
{
    [SerializeField] PoolableObject[] _weapons;
    [SerializeField] PoolableObject[] _cheatItems;

    public override void Initialize(RoomInfo roomInfo)
    {
        base.Initialize(roomInfo);
        RewardSelectionFinished();
        //CheatReward();
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