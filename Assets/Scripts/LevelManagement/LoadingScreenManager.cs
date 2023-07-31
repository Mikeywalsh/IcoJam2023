using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    public static bool IsTransitionActive;

    public Canvas LoadingScreenCanvas;
    public Camera LoadingScreenCamera;
    public Image FadeTransitionImage;
    private LoadingCubeManager _loadingCube;

    private const float FADE_TRANSITION_DURATION_SECONDS = 1f;
    private const float FADE_WAIT_BETWEEN_TRANSITIONS_DURATION_SECONDS = .5f;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning(
                $"Singleton instance of {nameof(LoadingScreenManager)} already exists! Deleting this one...");
            Destroy(gameObject);
            return;
        }
        _loadingCube = GetComponent<LoadingCubeManager>();
        Instance = this;
        IsTransitionActive = true;
    }

    private void Start()
    {
        LevelLoaderManager.Instance.FinishedLoadingLevel += OnFinishedLoadingLevel;
        OnFinishedLoadingLevel(this, EventArgs.Empty);
    }

    private void OnFinishedLoadingLevel(object sender, EventArgs e)
    {
        Instance.LoadingScreenCanvas.gameObject.SetActive(true);
        Instance.LoadingScreenCamera.gameObject.SetActive(true);
        SetFadeTransitionProgress(1f);
        IsTransitionActive = true;
    }

    public IEnumerator StartFadeTransition()
    {
        if (IsTransitionActive)
        {
            throw new Exception("Tried to start a cutout transition when one is already taking place!");
        }

        LoadingScreenCanvas.gameObject.SetActive(true);
        LoadingScreenCamera.gameObject.SetActive(true);
        IsTransitionActive = true;

        var currentDuration = 0f;

        while (currentDuration < FADE_TRANSITION_DURATION_SECONDS)
        {
            var currentProgress = Mathf.Lerp(0f, 1f, currentDuration / FADE_TRANSITION_DURATION_SECONDS);
            currentDuration += Time.deltaTime;
            SetFadeTransitionProgress(currentProgress);
            yield return null;
        }

        SetFadeTransitionProgress(1f);
    }

    public IEnumerator EndFadeTransition()
    {
        if (!IsTransitionActive)
        {
            throw new Exception("Tried to end a cutout transition when one is not active!");
        }

        yield return new WaitForSeconds(FADE_WAIT_BETWEEN_TRANSITIONS_DURATION_SECONDS);

        var currentDuration = 0f;

        while (currentDuration < FADE_TRANSITION_DURATION_SECONDS)
        {
            var currentProgress = Mathf.Lerp(1f, 0f, currentDuration / FADE_TRANSITION_DURATION_SECONDS);
            currentDuration += Time.deltaTime;
            SetFadeTransitionProgress(currentProgress);
            yield return null;
        }

        LoadingScreenCanvas.gameObject.SetActive(false);
        LoadingScreenCamera.gameObject.SetActive(false);
        IsTransitionActive = false;
    }


    private static void SetFadeTransitionProgress(float progress)
    {
        var newColor = new Color(Instance.FadeTransitionImage.color.r, Instance.FadeTransitionImage.color.g,
            Instance.FadeTransitionImage.color.b, progress);
        Instance.FadeTransitionImage.color = newColor;
        Instance._loadingCube.SetAlpha(progress);
    }
}