using TMPro;
using UnityEngine;

public class InformationText : MonoBehaviour
{
    public TextMeshPro TextHolder;

    public void DisplayText(string text)
    {
        TextHolder.text = text;
    }
}