using Unity.Mathematics;
using UnityEngine;

public abstract class PlayerTemporal : Temporal<PlayerTemporalState>
{
    public Transform[] AllTransforms;


    protected override PlayerTemporalState GetState()
    {
        var allTransformStates = new SpatialTemporalState[AllTransforms.Length];

        for (var i = 0; i < AllTransforms.Length; i++)
        {
            allTransformStates[i] = new SpatialTemporalState(AllTransforms[i].position,
                AllTransforms[i].rotation.eulerAngles, AllTransforms[i].localScale);
        }

        return new PlayerTemporalState(allTransformStates);
    }

    protected override void SetState(PlayerTemporalState state)
    {
        for (var i = 0; i < state.AllTransformSpatialStates.Length; i++)
        {
            AllTransforms[i].position = state.AllTransformSpatialStates[i].Position;
            AllTransforms[i].rotation = Quaternion.Euler(state.AllTransformSpatialStates[i].Rotation);
            AllTransforms[i].localScale = state.AllTransformSpatialStates[i].Scale;
        }
        
    }

    public override PlayerTemporalState[] CopyBuffer()
    {
        var copiedBuffer = new PlayerTemporalState[TemporalManager.MAX_LEVEL_FRAMES];

        for (var i = 0; i < TemporalBuffer.Length; i++)
        {
            if (TemporalBuffer[i] == null)
                continue;

            copiedBuffer[i] = TemporalBuffer[i].CloneState();
        }

        return copiedBuffer;
    }

    public override void OnInteractedWith()
    {
        // Players can not be interacted with
        return;
    }
}