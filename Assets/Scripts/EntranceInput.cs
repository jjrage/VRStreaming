using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntranceInput : MonoBehaviour
{
    public TMP_InputField inputField;

    public void SubmitEntrancePassword()
    {
        ReceivingTest.entrancePasswordSubmited?.Invoke(inputField.text);
    }
}
