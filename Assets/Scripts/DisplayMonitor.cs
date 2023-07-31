using TMPro;
using UnityEngine;

public class DisplayMonitor : MonoBehaviour
{
    private TextMeshPro _text;

    private string _templateText;
    
    public string[] ControllerArgs;
    public string[] KeyboardMouseArgs;

    private void Start()
    {
        _text = GetComponentInChildren<TextMeshPro>();
        _templateText = _text.text;
    }

    private void FixedUpdate()
    {
        var argsToUse = InputActionsManager.CurrentInputScheme == InputScheme.CONTROLLER ? ControllerArgs : KeyboardMouseArgs;
        _text.text = string.Format(_templateText, argsToUse);
    }
}