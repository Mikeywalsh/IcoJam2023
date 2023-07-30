using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DoubleJumpHash = Animator.StringToHash("DoubleJump");
    private static readonly int DashHash = Animator.StringToHash("Dash");

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void SetIsRunning(bool value)
    {
        _animator.SetBool(IsRunningHash, value);
    }
    
    public void Jump()
    {
        _animator.SetTrigger(JumpHash);
    }
    
    public void DoubleJump()
    {
        _animator.SetTrigger(DoubleJumpHash);
    }
    
    public void Dash()
    {
        _animator.SetTrigger(DashHash);
    }

    public void PauseAnimations()
    {
        _animator.speed = 0;
    }
    
    public void ResumeAnimations()
    {
        _animator.speed = 1;
    }
}