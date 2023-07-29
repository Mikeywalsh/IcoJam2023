using UnityEngine;

public abstract class Temporal<T> : MonoBehaviour, ITemporal where T : TemporalState
{
    private const int BUFFER_SIZE = TemporalManager.MAX_LEVEL_FRAMES;

    private readonly T[] _temporalBuffer = new T[BUFFER_SIZE];

    protected int CurrentFrame;
    protected int LockedEnd;

    protected virtual void Start()
    {
    }
    
    public void UpdateTemporalState()
    {
        if (LockedEnd > CurrentFrame)
        {
            SetState(_temporalBuffer[CurrentFrame]);
        }
        else
        {
            _temporalBuffer[CurrentFrame] = GetState();
        }

        CurrentFrame++;
    }

    public void ResetTemporal()
    {
        SetState(_temporalBuffer[0]);
        CurrentFrame = 0;
    }

    protected abstract T GetState();
    protected abstract void SetState(T state);

    public void OnInteractedWith()
    {
        LockedEnd = CurrentFrame;
    }
}