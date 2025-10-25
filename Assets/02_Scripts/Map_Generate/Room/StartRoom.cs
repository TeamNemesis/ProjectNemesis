public class StartRoom : Room
{
    public override void Initialize(RoomInfo roomInfo)
    {
        base.Initialize(roomInfo);
        RewardSelectionFinished();
    }

    public override RewardInteractableObject[] SpawnReward()
    {
        throw new System.NotImplementedException();
    }
}