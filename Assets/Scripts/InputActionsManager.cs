using UnityEngine;
using UnityEngine.EventSystems;

public class InputActionsManager : MonoBehaviour
{
    private static InputActionsManager _instance;

    public static InputActions InputActions;
    public static bool IsMouseOverUIElement => _instance.EventSystem.IsPointerOverGameObject();

    public EventSystem EventSystem;

    private Vector2 _mousePosition;

    public static Vector2 GetMousePosition => _instance._mousePosition;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning(
                $"Singleton instance of {nameof(InputActionsManager)} already exists! Deleting this one...");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        InputActions = new InputActions();
    }

    private void Start()
    {
        InputActions.Camera.MousePosition.performed += ctx => _mousePosition = ctx.ReadValue<Vector2>();
        InputActions.Camera.MousePosition.canceled += _ => _mousePosition = Vector2.zero;
    }

    private void OnEnable()
    {
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    public void EnableInput()
    {
        InputActions.Enable();
    }

    public void DisableInput()
    {
        InputActions.Disable();
    }
}