using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputActionsManager : MonoBehaviour
{
    public static InputActionsManager Instance;

    public static InputActions InputActions;
    public static bool IsMouseOverUIElement => Instance.EventSystem.IsPointerOverGameObject();

    public EventSystem EventSystem;

    private Vector2 _mousePosition;

    public static Vector2 GetMousePosition => Instance._mousePosition;

    public static InputScheme CurrentInputScheme;

    private static InputDevice _lastInputDevice;

    public EventHandler<InputSchemeChangedEventArgs> InputSchemeChanged;

    private void Awake()
    {
        Instance = this;
        InputActions = new InputActions();

        InputActions.Camera.MousePosition.performed += ctx =>
        {
            InputDeviceUsed(ctx.control.device);
            _mousePosition = ctx.ReadValue<Vector2>();
        };
        InputActions.Camera.MousePosition.canceled += _ => { _mousePosition = Vector2.zero; };
        InputActions.Camera.CameraDragButton.performed += ctx => { InputDeviceUsed(ctx.control.device); };
        InputActions.Camera.CameraDragDirection.performed += ctx =>
        {
            InputDeviceUsed(ctx.control.device);
        };

        InputActions.Player.Move.performed += ctx => InputDeviceUsed(ctx.control.device);
        InputActions.Player.Jump.started += ctx => InputDeviceUsed(ctx.control.device);
        InputActions.Player.Dash.started += ctx => InputDeviceUsed(ctx.control.device);
        InputActions.Player.Reverse.started += ctx => InputDeviceUsed(ctx.control.device);
        InputActions.Player.ResetLevel.started += ctx => InputDeviceUsed(ctx.control.device);

        if (CurrentInputScheme == InputScheme.UNSET)
        {
            OnInputSchemeChanged(InputScheme.MOUSE_KEYBOARD);
        }
    }

    private void InputDeviceUsed(InputDevice device)
    {
        var previousInputScheme = CurrentInputScheme;
        if (device is Gamepad)
        {
            CurrentInputScheme = InputScheme.CONTROLLER;
        }
        else
        {
            CurrentInputScheme = InputScheme.MOUSE_KEYBOARD;
        }

        if (previousInputScheme != CurrentInputScheme)
        {
            OnInputSchemeChanged(CurrentInputScheme);
        }
    }

    private void OnInputSchemeChanged(InputScheme inputScheme)
    {
        var eventArgs = new InputSchemeChangedEventArgs(inputScheme);
        InputSchemeChanged?.Invoke(this, eventArgs);
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