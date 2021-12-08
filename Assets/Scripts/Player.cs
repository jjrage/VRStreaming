using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Public Fields
    #endregion

    #region Editor Fields
    #endregion

    #region Private Fields
    [SerializeField]
    private Transform m_keyboardRoot;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        Security.OnEnterRequest += OpenKeyboard;
        ReceivingTest.OnPlayerSubmitedCorrectPassword += CloseKeyboard;
        EntranceInput.OnKeyboardExit += CloseKeyboard;
    }

    private void CloseKeyboard()
    {
        m_keyboardRoot.gameObject.SetActive(false);
    }

    private void OpenKeyboard()
    {
        m_keyboardRoot.gameObject.SetActive(true);
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion
}
