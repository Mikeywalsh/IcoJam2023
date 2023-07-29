﻿using System;

public class PastPlayerTemporal : PlayerTemporal
{
    // If the player presses the reset button early, we need this so we can despawn the past player in the same frame
    private int _lastFrame;

    public void Initialize(SpatialTemporalState[] playerBuffer, int lastFrame)
    {
        TemporalBuffer = playerBuffer;
        _lastFrame = lastFrame;
        
        // Past players are always locked, we only read from buffer
        LockedEnd = TemporalManager.MAX_LEVEL_FRAMES - 1;
    }
    
    protected override SpatialTemporalState GetState()
    {
        // No logic here, past player should never write its state to buffer
        throw new NotSupportedException();
    }

    protected override void SetState(SpatialTemporalState state)
    {
        if (CurrentFrame >= _lastFrame - 1)
        {
            // Remove this past player
            SetActive(false);
            return;
        }
        
        base.SetState(state);
    }
}