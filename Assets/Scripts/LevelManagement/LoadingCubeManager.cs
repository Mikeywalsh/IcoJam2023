using UnityEngine;
using UnityEngine.UI;

public class LoadingCubeManager : MonoBehaviour
{
    public RawImage LoadingCubeImage;
    public Camera LoadingCubeCamera;
    public Transform LoadingCube;

    private bool _isLoading;
    private Vector3 _rotateAxis;

    private void Awake()
    {
        _rotateAxis = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }

    private void Start()
    {
        LevelLoaderManager.Instance.StartedLevelExit += OnStartedLoading;
        LevelLoaderManager.Instance.FinishedLoadingLevel += OnStartedLoading;
        LevelLoaderManager.Instance.FinishedLevelIntroTransition += OnFinishedLoading;
    }

    public void SetAlpha(float progress)
    {
        var loadingCubeImageColor = LoadingCubeImage.color;

        float newAlpha;

        if (progress <= 0.4f)
        {
            newAlpha = 0f;
        }
        else
        {
            newAlpha = (progress - 0.4f) * (10f / 6f);
        }

        loadingCubeImageColor.a = newAlpha;
        LoadingCubeImage.color = loadingCubeImageColor;
    }

    private void FixedUpdate()
    {
        if (!_isLoading)
            return;

        _rotateAxis = Quaternion.AngleAxis(4f,
            new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f))) * _rotateAxis;

        var anglesThisFrame = Random.Range(2f, 4f);
        LoadingCube.Rotate(_rotateAxis, anglesThisFrame);
    }

    public void OnStartedLoading(object sender, System.EventArgs args)
    {
        _isLoading = true;
        LoadingCubeCamera.gameObject.SetActive(true);
        LoadingCube.gameObject.SetActive(true);
        LoadingCubeImage.enabled = true;
    }

    public void OnFinishedLoading(object sender, System.EventArgs args)
    {
        _isLoading = false;
        LoadingCubeCamera.gameObject.SetActive(false);
        LoadingCube.gameObject.SetActive(false);
        LoadingCubeImage.enabled = false;
    }
}