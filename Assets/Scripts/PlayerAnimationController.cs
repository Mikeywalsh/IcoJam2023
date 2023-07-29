using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void SetIsRunning(bool value)
    {
        _animator.SetBool(IsRunningHash, value);
    }
}