public class ColosseumRoom : Room
{
    public override IInteractable[] SpawnReward()
    {
        throw new System.NotImplementedException();
    }

    private void OnDisable()
    {
        EventBus.SetColosseumRoom(false);
    }
}