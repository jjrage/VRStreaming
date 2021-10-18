using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadialButton : MonoBehaviour
{
    #region Public Fields
    #endregion

    #region Editor Fields
    [SerializeField]
    private TMP_Text m_text;
    [SerializeField]
    private Button m_button;
    #endregion

    #region Private Fields
    #endregion

    #region MonoBehaviour
    private void OnEnable()
    {

    }

    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void OnDisable()
    {

    }
    #endregion

    #region Public Methods
    public void InitButton(Action buttonAction, string buttonText)
    {
        m_text.text = buttonText;
        m_button.onClick.AddListener(() => buttonAction?.Invoke());
    }
    #endregion

    #region Private Methods
    #endregion
}
