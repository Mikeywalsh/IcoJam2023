using System;

public class PresentPlayerTemporal : PlayerTemporal
{
    protected override void SetState(SpatialTemporalState state)
    {
        // Present players should only have their state set on the very first frame upon level reset
        if (CurrentFrame == 0)
        {
            // TODO - Cancel any actions/animations at this point too
            base.SetState(state);
            return;
        }
        throw new NotSupportedException();
    }
}