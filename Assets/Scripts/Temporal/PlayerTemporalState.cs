public record PlayerTemporalState : TemporalState
{
    public SpatialTemporalState[] AllTransformSpatialStates { get; }

    public PlayerTemporalState(SpatialTemporalState[] allTransformSpatialStates)
    {
        AllTransformSpatialStates = allTransformSpatialStates;
    }

    public PlayerTemporalState CloneState()
    {
        var copiedBuffer = new SpatialTemporalState[AllTransformSpatialStates.Length];

        for (var i = 0; i < AllTransformSpatialStates.Length; i++)
        {
            if (AllTransformSpatialStates[i] == null)
                continue;

            copiedBuffer[i] = AllTransformSpatialStates[i].CloneState();
        }

        return new PlayerTemporalState(copiedBuffer);
    }
}