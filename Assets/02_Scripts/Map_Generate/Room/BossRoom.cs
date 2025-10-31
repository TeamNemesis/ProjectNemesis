public class BossRoom : Room
{
    public override IInteractable[] SpawnReward()
    {
        // BossRoom은 보상을 생성하지 않음
        return System.Array.Empty<IInteractable>();
    }
}