using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class TemporalManager : MonoBehaviour
{
    public const int FRAMES_PER_SECOND = 50;
    public const int LEVEL_LENGTH_SECONDS = 60;
    public const int MAX_LEVEL_FRAMES = FRAMES_PER_SECOND * LEVEL_LENGTH_SECONDS;

    private const int SLOWDOWN_FRAMES = 50;
    private const int SLOWDOWN_START = MAX_LEVEL_FRAMES - SLOWDOWN_FRAMES;

    public PastPlayerTemporal PastPlayerTemporalPrefab;
    private PresentPlayerTemporal _presentPlayer;

    private int _currentFrame;
    private int _frameAtReset;
    private List<ITemporal> _allTemporals;
    private bool _reversing;

    private CameraControl _cameraControl;

    private void Start()
    {

        _presentPlayer = FindObjectOfType<PresentPlayerTemporal>();
        _cameraControl = FindObjectOfType<CameraControl>();
        _allTemporals = new List<ITemporal>();
        var temporalTagGameObjects = GameObject.FindGameObjectsWithTag("Temporal");

        foreach (var temporalTagGameObject in temporalTagGameObjects)
        {
            var temporalGameObject = temporalTagGameObject.GetComponent<ITemporal>();
            if (temporalGameObject == null)
            {
                throw new Exception(
                    $"Gameobject: {temporalTagGameObject.name} was expected to have the temporal tag, but it didn't...");
            }

            _allTemporals.Add(temporalGameObject);
        }

        _allTemporals = _allTemporals.OrderBy(temporal => temporal.ExecutionOrder()).ToList();
        InputActionsManager.InputActions.Player.Reverse.started += _ => StartReset();
    }

    private void FixedUpdate()
    {
        if (_reversing)
            return;

        var slowDownReached = _currentFrame >= SLOWDOWN_START - 1;

        if (slowDownReached)
        {
            var framesIntoSlowDown = _currentFrame - SLOWDOWN_START - 1;

            var newTimeScale = math.lerp(1, .2f, framesIntoSlowDown / (float) SLOWDOWN_FRAMES);
            Time.timeScale = newTimeScale;
        }

        var levelEndReached = _currentFrame >= MAX_LEVEL_FRAMES - 1;
        if (levelEndReached)
        {
            Time.timeScale = 0;
            return;
        }

        foreach (var temporal in _allTemporals)
        {
            temporal.UpdateTemporalState(_currentFrame, false);
        }

        _currentFrame++;
    }

    private void OnReverseFinished()
    {
        // Create past player
        var presentPlayerBufferCopy = _presentPlayer.CopyBuffer();
        var pastPlayer = Instantiate(PastPlayerTemporalPrefab, Vector3.zero, quaternion.identity);
        pastPlayer.Initialize(presentPlayerBufferCopy, _frameAtReset);
        _allTemporals.Add(pastPlayer);

        foreach (var temporal in _allTemporals)
        {
            temporal.SetActive(true);
            temporal.ResetTemporal();
        }

        _reversing = false;
        _currentFrame = 0;
    }

    public void StartReset()
    {
        if (_currentFrame == 0 || _reversing)
        {
            return;
        }
        
        Time.timeScale = 1;
        _reversing = true;
        _currentFrame -= 1;
        _frameAtReset = _currentFrame;
        
        foreach (var temporal in _allTemporals)
        {
            temporal.StartedReversing();
        }

        _presentPlayer.GetComponent<PlayerController>().DisableInputAndAnimations();
        var timeToReverse = 2f;
        
        _cameraControl.OnLevelReverse(timeToReverse);
        DOTween.To(() => _currentFrame, x => _currentFrame = x, 0, timeToReverse)
            .OnUpdate(() =>
            {
                foreach (var temporal in _allTemporals)
                {
                    temporal.UpdateTemporalState(_currentFrame, true);
                }
            })
            .SetEase(Ease.OutQuad)
            .SetDelay(0.4f)
            .OnComplete(OnReverseFinished);
    }
}