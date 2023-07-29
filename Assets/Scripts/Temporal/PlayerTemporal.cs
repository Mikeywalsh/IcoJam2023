public abstract class PlayerTemporal : SpatialTemporal
{
    
    public override void OnInteractedWith()
    {
        // Players can not be interacted with
        return;
    }
}