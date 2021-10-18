using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Security : MonoBehaviour
{
    #region Public Fields
    #endregion

    #region Editor Fields
    [SerializeField]
    private XRSimpleInteractable m_interactable;
    #endregion

    #region Private Fields
    private Dictionary<Action, string> m_securityActions;
    #endregion

    #region MonoBehaviour
    private void OnEnable()
    {
        
    }

    private void Awake()
    {
        m_interactable.selectEntered.AddListener(SecuritySelected);
        m_interactable.selectExited.AddListener(SecuritDeselected);
        m_securityActions = CreateSecurityActions();
    }

    private Dictionary<Action, string> CreateSecurityActions()
    {
        Dictionary<Action, string> actions = new Dictionary<Action, string>();
        actions.Add(ClubInformationAction, "Info");
        actions.Add(FaceControllAction, "Enter");
        return actions;
    }

    private void SecuritDeselected(SelectExitEventArgs arg0)
    {
        RadialMenu.RemoveActions();
    }

    private void SecuritySelected(SelectEnterEventArgs arg0)
    {
        Debug.Log("Security selected");
        RadialMenu.SetActions(m_securityActions);
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
    #endregion

    #region Private Methods
    private void FaceControllAction()
    {
        Debug.Log("Face controll action proceeded");
    }

    private void ClubInformationAction()
    {
        Debug.Log("Club information action proceeded");
    }
    #endregion
}
