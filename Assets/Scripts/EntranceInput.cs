using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class EntranceInput : MonoBehaviour
{
    public TMP_InputField inputField;

    public delegate void ReturnEvent(string text);
    public static ReturnEvent OnReturnPressed;
    public delegate void BackspaceEvent();
    public static BackspaceEvent OnBackspacePressed;

    public void OnEnterPressed()
    {
        OnReturnPressed?.Invoke(inputField.text);
        inputField.text = string.Empty;
    }

    public void OnEscPressed()
    {
        OnBackspacePressed?.Invoke();
        inputField.text = string.Empty;
    }
}
