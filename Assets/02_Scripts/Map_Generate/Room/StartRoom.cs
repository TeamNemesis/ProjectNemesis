using UnityEngine;
using UnityEngine.Rendering;

public class StartRoom : Room
{
    public override void Initialize(RoomInfo roomInfo)
    {
        base.Initialize(roomInfo);
        //CheatReward();
    }

    public override IInteractable[] SpawnReward()
    {
        TechSelectPackType techSelectPackType = GameManager.Instance.skillManager.GetSkillPackTypes(1)[0];

        GameObject techPackPrefab = GameManager.Instance.PoolManager.GetFromPool(
            Constants.RESOURCES_PATH_REWARDS + $"/TechSelectPack_Company{(int)techSelectPackType + 1}",
            _rewardSpawnPoints[0].position,
            Quaternion.identity);

        RewardInteractableObject rewardInteractableObject = techPackPrefab.GetComponent<RewardInteractableObject>();
        rewardInteractableObject.OnRewardGiven += RewardSelectionFinished;
        rewardInteractableObject.Initialize();

        return new IInteractable[] { techPackPrefab.GetComponent<IInteractable>() };
    }
}