using System;
using UnityEngine;

public class PastPlayerTemporal : PlayerTemporal
{
    [SerializeField]
    private PastPlayerResetEffect _pastPlayerResetEffect;
    
    // If the player presses the reset button early, we need this so we can despawn the past player in the same frame
    private int _lastFrame;

    public void Initialize(PlayerTemporalState[] playerBuffer, int lastFrame)
    {
        TemporalBuffer = playerBuffer;
        _lastFrame = lastFrame;
        
        // Past players are always locked, we only read from buffer
        LockedEnd = TemporalManager.MAX_LEVEL_FRAMES - 1;
    }
    
    protected override PlayerTemporalState GetState()
    {
        // No logic here, past player should never write its state to buffer
        throw new NotSupportedException();
    }

    protected override void SetState(PlayerTemporalState state)
    {
        if (CurrentFrame >= _lastFrame - 1)
        {
            if (!Reversing)
            {
                // Remove this past player and spawn effect
                Instantiate(_pastPlayerResetEffect, transform.position, transform.rotation);
                SetActive(false);
            }
            return;
        }
        
        if (CurrentFrame < _lastFrame && Reversing)
        {
            SetActive(true);
        }

        if (gameObject.activeSelf)
        {
            base.SetState(state);
        }
    }
}