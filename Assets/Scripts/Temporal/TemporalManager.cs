using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TemporalManager : MonoBehaviour
{
    public const int FRAMES_PER_SECOND = 50;
    public const int LEVEL_LENGTH_SECONDS = 60;
    public const int MAX_LEVEL_FRAMES = FRAMES_PER_SECOND * LEVEL_LENGTH_SECONDS;

    public PastPlayerTemporal PastPlayerTemporalPrefab;
    private PresentPlayerTemporal _presentPlayer;

    private int _currentFrame;
    private List<ITemporal> _allTemporals;
    
    private void Start()
    {
        _presentPlayer = FindObjectOfType<PresentPlayerTemporal>();
        _allTemporals = new List<ITemporal>();
        var temporalTagGameObjects = GameObject.FindGameObjectsWithTag("Temporal");
        
        foreach (var temporalTagGameObject in temporalTagGameObjects)
        {
            var temporalGameObject = temporalTagGameObject.GetComponent<ITemporal>();
            if (temporalGameObject == null)
            {
                throw new Exception($"Gameobject: {temporalTagGameObject.name} was expected to have the temporal tag, but it didn't...");
            }
            _allTemporals.Add(temporalGameObject);
        }
    }
    
    private void FixedUpdate()
    {
        var levelEndReached = _currentFrame >= MAX_LEVEL_FRAMES - 1;
        if (levelEndReached)
        {
            Time.timeScale = 0;
            return;
        }
        
        foreach (var temporal in _allTemporals)
        {
            temporal.UpdateTemporalState();
        }

        _currentFrame++;
    }

    public void Reset()
    {
        // Create past player
        var presentPlayerBufferCopy = _presentPlayer.CopyBuffer();
        var pastPlayer = Instantiate(PastPlayerTemporalPrefab, Vector3.zero, quaternion.identity);
        pastPlayer.Initialize(presentPlayerBufferCopy, _currentFrame);
        _allTemporals.Add(pastPlayer);
        
        foreach (var temporal in _allTemporals)
        {
            temporal.SetActive(true);
            temporal.ResetTemporal();
        }
        
        Time.timeScale = 1;
        _currentFrame = 0;
    }
}