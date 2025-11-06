using UnityEngine;

public class LabRoom : Room
{
    public override IInteractable[] SpawnReward()
    {
        GameObject obj = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Rewards/MutantPack", _rewardSpawnPoints[0].position, Quaternion.identity);
        obj.transform.localScale = Vector3.one * 4f; // 랩실 보상은 크기를 키워서 스폰
        return new IInteractable[] { obj.GetComponent<IInteractable>() };
    }
}   