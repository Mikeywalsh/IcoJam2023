using UnityEngine;
using Random = UnityEngine.Random;

public class VictoryCube : MonoBehaviour
{
    public Transform CubeModel;
    private Vector3 _rotateAxis;

    private void Start()
    {
        _rotateAxis = Vector3.up;
    }

    private void FixedUpdate()
    {
        _rotateAxis = Quaternion.AngleAxis(4f,
            new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f))) * _rotateAxis;

        var anglesThisFrame = Random.Range(2f, 4f);
        CubeModel.Rotate(_rotateAxis, anglesThisFrame);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PresentPlayerTemporal>() == null)
        {
            return;
        }
        
        AudioManager.Play("cube-collect");
        LevelLoaderManager.MoveToNextLevel();
    }
}