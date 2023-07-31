using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoaderManager : MonoBehaviour
{
    public static LevelLoaderManager Instance;
    public bool IsLevelLoaded;

    public EventHandler StartedLevelExit;
    public EventHandler FinishedLoadingLevel;
    public EventHandler FinishedLevelIntroTransition;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning(
                $"Singleton instance of {nameof(LevelLoaderManager)} already exists! Deleting this one...");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
    }

    private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(StartLevelLoadedTransition());
    }


    private IEnumerator StartLevelLoadedTransition()
    {
        OnFinishedLoadingLevel();
        yield return LoadingScreenManager.Instance.EndFadeTransition();
        IsLevelLoaded = true;
        OnFinishedLevelIntroTransition();
    }
    
    public static void MoveToLevel(int sceneId)
    {
        Instance.StartCoroutine(Instance.StartLevelExit(sceneId));
    }
    
    private IEnumerator StartLevelExit(int sceneId)
    {
        IsLevelLoaded = false;
        
        OnStartedLevelExit();
        yield return LoadingScreenManager.Instance.StartFadeTransition();
        yield return SceneManager.LoadSceneAsync(sceneId);
    }

    protected virtual void OnStartedLevelExit()
    {
        StartedLevelExit?.Invoke(this, EventArgs.Empty);
    }
    
    protected virtual void OnFinishedLevelIntroTransition()
    {
        FinishedLevelIntroTransition?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnFinishedLoadingLevel()
    {
        FinishedLoadingLevel?.Invoke(this, EventArgs.Empty);
    }
}
