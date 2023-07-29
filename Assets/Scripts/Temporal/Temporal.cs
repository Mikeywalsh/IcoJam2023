using UnityEngine;

public abstract class Temporal<T> : MonoBehaviour, ITemporal where T : TemporalState
{
    protected T[] TemporalBuffer = new T[TemporalManager.MAX_LEVEL_FRAMES];
    protected int CurrentFrame;
    protected int LockedEnd = -1;

    protected bool Reversing;

    public virtual int ExecutionOrder() => 0;

    protected virtual void Start()
    {
    }
    
    public void UpdateTemporalState(int currentFrame, bool reversing)
    {
        Reversing = reversing;
        CurrentFrame = currentFrame;
        
        if (!gameObject.activeSelf && !reversing)
        {
            return;
        }
        
        if (IsLocked() || reversing)
        {
            SetState(TemporalBuffer[CurrentFrame]);
        }
        else
        {
            TemporalBuffer[CurrentFrame] = GetState();
        }

    }

    public bool IsLocked() => LockedEnd > CurrentFrame;

    public virtual void ResetTemporal()
    {
        CurrentFrame = 0;
        Reversing = false;
        SetState(TemporalBuffer[0]);
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    protected abstract T GetState();
    protected abstract void SetState(T state);

    public virtual void OnInteractedWith()
    {
        LockedEnd = CurrentFrame;
    }

    public virtual T[] CopyBuffer()
    {
        var copiedBuffer = new T[TemporalManager.MAX_LEVEL_FRAMES];
        TemporalBuffer.CopyTo(copiedBuffer, 0);
        return copiedBuffer;
    }
}