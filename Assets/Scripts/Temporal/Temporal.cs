using UnityEngine;

public abstract class Temporal<T> : MonoBehaviour, ITemporal where T : TemporalState
{
    protected T[] TemporalBuffer = new T[TemporalManager.MAX_LEVEL_FRAMES];
    protected int CurrentFrame;
    protected int LockedEnd = -1;

    private bool _reversing;

    protected virtual void Start()
    {
    }
    
    public void UpdateTemporalState(int currentFrame, bool reversing)
    {
        _reversing = reversing;
        
        if (!gameObject.activeSelf)
        {
            return;
        }
        
        if (LockedEnd > CurrentFrame || reversing)
        {
            SetState(TemporalBuffer[CurrentFrame]);
        }
        else
        {
            TemporalBuffer[CurrentFrame] = GetState();
        }

        CurrentFrame = currentFrame;
    }

    public void ResetTemporal()
    {
        CurrentFrame = 0;
        _reversing = false;
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

    public T[] CopyBuffer()
    {
        var copiedBuffer = new T[TemporalManager.MAX_LEVEL_FRAMES];
        TemporalBuffer.CopyTo(copiedBuffer, 0);
        return copiedBuffer;
    }
}