using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _mainCamera;

    void Start()
    {
        _mainCamera = FindObjectOfType<CameraControl>().transform;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.position, Vector3.up);
    }
}
