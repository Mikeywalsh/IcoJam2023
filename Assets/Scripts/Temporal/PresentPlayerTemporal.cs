using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PresentPlayerTemporal : PlayerTemporal
{
    private PlayerController _playerController;

    protected override void Start()
    {
        base.Start();

        _playerController = GetComponent<PlayerController>();
    }

    public override void ResetTemporal()
    {
        base.ResetTemporal();
        _playerController.OnResetTemporal();
    }

    protected override void SetState(PlayerTemporalState state)
    {
        // Present players should only have their state set on the very first frame upon level reset or when reversing
        if (CurrentFrame == 0 || Reversing)
        {
            _playerController.DisableInputAndAnimations();
            base.SetState(state);
            return;
        }
        throw new NotSupportedException();
    }
}