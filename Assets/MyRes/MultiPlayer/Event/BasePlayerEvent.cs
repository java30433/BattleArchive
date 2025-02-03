abstract class BasePlayerEvent {
    public byte SenderId;
    public void Apply()
    {
        MultiPlayerManager.Instance.GetPlayer(SenderId).EventList.Add(this);
    }
    public abstract void Dispath(PlayerStat player);
}