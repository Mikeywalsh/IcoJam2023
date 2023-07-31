using System;
using UnityEngine;

public class PastPlayerTemporal : PlayerTemporal
{
    [SerializeField]
    private PastPlayerResetEffect _pastPlayerResetEffect;
    
    [SerializeField]
    private PastPlayerResetEffect _pastPlayerDeathEffect;
    
    // If the player presses the reset button early, we need this so we can despawn the past player in the same frame
    private int _lastFrame;


    private bool _playerDies;
    
    public override void Initialize(int bufferSize)
    {
        // Past players are always locked, we only read from buffer
        LockedEnd = bufferSize - 1;
    }

    public void SetPastPlayerData(PlayerTemporalState[] playerBuffer, int lastFrame)
    {
        TemporalBuffer = playerBuffer;
        _lastFrame = lastFrame;
    }
    
    protected override PlayerTemporalState GetState()
    {
        // No logic here, past player should never write its state to buffer
        throw new NotSupportedException();
    }

    protected override void SetState(PlayerTemporalState state)
    {
        if (!Reversing && state.StartedDying && !_playerDies)
        {
            // Remove this past player and spawn effect
            Instantiate(_pastPlayerDeathEffect, transform.position, transform.rotation);
            _playerDies = true;
        }
        
        if (CurrentFrame >= _lastFrame - 1)
        {
            if (!Reversing && !_playerDies)
            {
                // Remove this past player and spawn effect
                Instantiate(_pastPlayerResetEffect, transform.position, transform.rotation);
                
            }
            if (!Reversing)
            {
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