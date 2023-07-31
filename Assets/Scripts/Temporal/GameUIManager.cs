using System.Globalization;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public TextMeshProUGUI SecondsLeftText;
    
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
    }
}