using UnityEngine;

public record SpatialTemporalState : TemporalState
{
    public Vector3 Position { get; }
    public Vector3 Rotation { get; }
    public Vector3 Scale { get; }

    public SpatialTemporalState(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}