using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class EntranceInput : MonoBehaviour
{
    public TMP_InputField inputField;

    public delegate void ReturnEvent(string text);
    public static ReturnEvent OnPasswordSubmited;
    public delegate void BackspaceEvent();
    public static BackspaceEvent OnKeyboardExit;

    public void SubmitEntrancePassword()
    {
        Debug.Log("SubmitEntrancePassword");
        OnPasswordSubmited?.Invoke(inputField.text);
        inputField.text = string.Empty;
    }

    public void ExitKeyboard()
    {
        OnKeyboardExit?.Invoke();
        inputField.text = string.Empty;
    }
}
