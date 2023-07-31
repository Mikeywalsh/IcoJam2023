using System;
using System.Globalization;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public TextMeshProUGUI SecondsLeftText;
    public TextMeshProUGUI RewindsLeftText;
    public TextMeshProUGUI RewindText;
    public GameObject EndOfLevelText;
    
    public TextMeshProUGUI RewindReminderText;
    public TextMeshProUGUI RestartReminderText;

    public void SetFrame(int currentFrame, int maxFrames)
    {
        var framesLeft = maxFrames - currentFrame;
        var secondsLeft = (framesLeft / (float) TemporalManager.FRAMES_PER_SECOND) + 1;


        var baseScale = Vector3.one * 0.7f;
        SecondsLeftText.rectTransform.localScale =
            baseScale + (baseScale * (0.5f * (secondsLeft - math.floor(secondsLeft))));

        var shouldShow = LevelLoaderManager.Instance.IsLevelLoaded &&
                         math.floor(secondsLeft) <=
                         Mathf.RoundToInt(maxFrames / (float) TemporalManager.FRAMES_PER_SECOND);

        string textToDisplay;

        if (currentFrame >= maxFrames - 1)
        {
            textToDisplay = "Time's up!";
        }
        else
        {
            textToDisplay= math.floor(secondsLeft).ToString(CultureInfo.InvariantCulture);
        }

        SecondsLeftText.text = textToDisplay;
        SecondsLeftText.gameObject.SetActive(shouldShow);
    }

    private void Update()
    {
        bool shouldDisplayRewindsLeft = LevelLoaderManager.Instance != null && LevelLoaderManager.Instance.IsLevelLoaded && LevelLoaderManager.CurrentLevelId != 0;

        if (InputActionsManager.CurrentInputScheme == InputScheme.MOUSE_KEYBOARD)
        {
            RestartReminderText.text = "Press 'L' to restart";
            RewindReminderText.text = "Press 'R' to rewind";
        }
        else
        {
            RestartReminderText.text = "Press 'B' to restart";
            RewindReminderText.text = "Press 'Left Trigger' to rewind";
        }
        
        RewindsLeftText.gameObject.SetActive(shouldDisplayRewindsLeft);
        RewindText.gameObject.SetActive(shouldDisplayRewindsLeft);
    }

    public void HideReminderText()
    {
        // Hack
        SecondsLeftText.text = string.Empty;
        
        RestartReminderText.gameObject.SetActive(false);
        RewindReminderText.gameObject.SetActive(false);
    }
    
    public void ShowReminderText(bool playerDied)
    {
        if (playerDied)
        {
            SecondsLeftText.gameObject.SetActive(true);
            SecondsLeftText.text = "You died!";
        }

        RestartReminderText.gameObject.SetActive(true);
        RewindReminderText.gameObject.SetActive(LevelLoaderManager.CurrentLevelId != 0);
    }
    
    public void UpdateRewindCount(int rewindCount)
    {
        if (rewindCount == 1)
        {
            RewindText.text = "Rewind";
        }
        else
        {
            RewindText.text = "Rewinds";
        }
        RewindsLeftText.text = rewindCount.ToString();
    }

    public void DisplayEndOfLevelText()
    {
        EndOfLevelText.SetActive(true);
    }

    public void HideEndOfLevelText()
    {
        EndOfLevelText.SetActive(true);
    }
}