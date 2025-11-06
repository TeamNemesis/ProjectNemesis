using UnityEngine;

public class LabRoom : Room
{
    public override IInteractable[] SpawnReward()
    {
        GameObject obj = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Rewards/MutantPack", _rewardSpawnPoints[0].position, Quaternion.identity);
        return new IInteractable[] { obj.GetComponent<IInteractable>() };
    }
}   