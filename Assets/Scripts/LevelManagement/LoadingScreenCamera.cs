using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace LevelManagement
{
    [RequireComponent(typeof(Camera))]
    public class LoadingScreenCamera : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            SceneManager.sceneLoaded += (arg0, mode) => OnStartedLoadingLevel();
            gameObject.SetActive(false);
            OnStartedLoadingLevel();
        }

        private void OnStartedLoadingLevel()
        {
            var mainCamera = FindObjectOfType<CameraControl>().GetComponent<Camera>();
            var cameraData = mainCamera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(_camera);
        }
    }
}