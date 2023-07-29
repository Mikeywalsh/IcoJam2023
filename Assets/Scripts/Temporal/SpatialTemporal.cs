using UnityEngine;

public class SpatialTemporal : Temporal<SpatialTemporalState>
{
    protected override SpatialTemporalState GetState()
    {
        return new(transform.position, transform.rotation.eulerAngles, transform.localScale);
    }

    protected override void SetState(SpatialTemporalState state)
    {
        transform.position = state.Position;
        transform.rotation = Quaternion.Euler(state.Rotation);
        transform.localScale = state.Scale;
    }
}