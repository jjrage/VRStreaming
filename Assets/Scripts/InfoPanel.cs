using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField]
    private Button m_exitButton;
    [SerializeField]
    private Transform m_container;

    private void Awake()
    {
        Security.OnGameInfo += OpenInfoPanel;
        m_exitButton.onClick.AddListener(CloseInfoPanel);
    }

    private void OpenInfoPanel()
    {
        m_container.gameObject.SetActive(true);
    }
    private void CloseInfoPanel()
    {
        m_container.gameObject.SetActive(false);
    }
}
