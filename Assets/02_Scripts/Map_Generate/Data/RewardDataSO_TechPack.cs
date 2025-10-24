using UnityEngine;

[CreateAssetMenu(fileName = "RewardDataSO", menuName = "ScriptableObjects/Reward/RewardDataSO_TechPack")]
public class RewardDataSO_TechPack : RewardDataSO
{
    [SerializeField] TechSelectPackType _techPackType;

    public TechSelectPackType TechPackType => _techPackType;
}