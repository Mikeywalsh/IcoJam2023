using System;
using UnityEngine;

public abstract class Temporal<T> : MonoBehaviour, ITemporal where T : TemporalState
{
    protected T[] TemporalBuffer = new T[TemporalManager.MAX_LEVEL_FRAMES];
    protected int CurrentFrame;
    protected int LockedEnd = -1;

    protected bool Reversing;
    
    // Needed to stop objects from writing when the 0.4 second pre-reverse animation begins
    private bool _hasStartedReversing;

    public Transform LockedIndicatorAnchor;
    public Transform InformationDisplayAnchor;
    public float LockedIndicatorScale = 0.5f;
    private GameObject _lockedIndicatorPrefab;
    private GameObject _informationTextPrefab;
    private GameObject _lockedIndicator;
    private InformationText _informationText;

    public virtual int ExecutionOrder() => 0;

    protected virtual void Start()
    {
        _lockedIndicatorPrefab = Resources.Load("LockedIndicator") as GameObject;
        _informationTextPrefab = Resources.Load("InformationText") as GameObject;

        if (LockedIndicatorAnchor == null)
        {
            Debug.Log($"Object: {gameObject.name} does not have a locked indicator anchor...");
        }
        else
        {
            _lockedIndicator = Instantiate(_lockedIndicatorPrefab, LockedIndicatorAnchor.position, Quaternion.identity);
            _lockedIndicator.transform.localScale = Vector3.one * LockedIndicatorScale;
            _lockedIndicator.transform.parent = LockedIndicatorAnchor;
            _lockedIndicator.SetActive(false);
        }


        if (InformationDisplayAnchor == null)
        {
            Debug.Log($"Object: {gameObject.name} does not have a information text anchor...");
        }
        else
        {
            _informationText = Instantiate(_informationTextPrefab, InformationDisplayAnchor.position, Quaternion.identity).GetComponent<InformationText>();
            _informationText.transform.localScale = Vector3.one * .35f;
            _informationText.transform.SetParent(InformationDisplayAnchor);
            _informationText.gameObject.SetActive(false);
        }
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

    public void StartedReversing()
    {
        _hasStartedReversing = true;
    }
    
    protected virtual void FixedUpdate()
    {
        if (_lockedIndicator != null)
        {
            _lockedIndicator.gameObject.SetActive(IsLocked() && !Reversing && !_hasStartedReversing);
        }
        
        if (_informationText != null)
        {
            _informationText.gameObject.SetActive(ShouldDisplayInformationText() && !Reversing && !_hasStartedReversing);
            _informationText.DisplayText(GetInformationText());
        }
    }

    protected virtual string GetInformationText()
    {
        return string.Empty;
    }

    protected virtual bool ShouldDisplayInformationText() => true;

    public bool IsLocked() => LockedEnd > CurrentFrame;

    public virtual void ResetTemporal()
    {
        CurrentFrame = 0;
        Reversing = false;
        _hasStartedReversing = false;
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