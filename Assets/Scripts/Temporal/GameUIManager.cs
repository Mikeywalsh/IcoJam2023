using System.Globalization;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public TextMeshProUGUI SecondsLeftText;
    public TextMeshProUGUI RewindsLeftText;
    public Image RewindIcon;
    
    public void SetFrame(int currentFrame, int maxFrames)
    {
        var framesLeft = maxFrames - currentFrame;
        var secondsLeft = (framesLeft / (float)TemporalManager.FRAMES_PER_SECOND) + 1;

        SecondsLeftText.text = math.floor(secondsLeft).ToString(CultureInfo.InvariantCulture);

        SecondsLeftText.rectTransform.localScale =
            Vector3.one + (Vector3.one * (0.5f * (secondsLeft - math.floor(secondsLeft))));

        var shouldShow = LevelLoaderManager.Instance.IsLevelLoaded &&
                         math.floor(secondsLeft) <= Mathf.RoundToInt(maxFrames / (float)TemporalManager.FRAMES_PER_SECOND) &&
                         currentFrame < maxFrames - 1;
        
        SecondsLeftText.gameObject.SetActive(shouldShow);
        RewindsLeftText.gameObject.SetActive(shouldShow);
        RewindIcon.gameObject.SetActive(shouldShow);
    }

    public void UpdateRewindCount(int rewindCount)
    {
        RewindsLeftText.text = "x" + rewindCount;
    }
}