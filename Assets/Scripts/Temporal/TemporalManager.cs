using System;
using System.Collections.Generic;
using UnityEngine;

public class TemporalManager : MonoBehaviour
{
    public const int FRAMES_PER_SECOND = 50;
    public const int LEVEL_LENGTH_SECONDS = 5;
    public const int MAX_LEVEL_FRAMES = FRAMES_PER_SECOND * LEVEL_LENGTH_SECONDS;

    private int _currentFrame;
    private List<ITemporal> _allTemporals;
    
    private void Start()
    {
        Debug.Log(Time.timeScale);
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
        var levelEndReached = _currentFrame >= MAX_LEVEL_FRAMES;
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
        foreach (var temporal in _allTemporals)
        {
            temporal.ResetTemporal();
        }
        Time.timeScale = 1;
        _currentFrame = 0;
    }
}