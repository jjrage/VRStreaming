using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
    #region Public Fields
    #endregion

    #region Editor Fields
    [SerializeField]
    private Transform m_root;
    [SerializeField]
    private Transform m_buttonsRoot;
    [SerializeField]
    private GameObject m_buttonPrefab;
    #endregion

    #region Private Fields
    private static Action<Dictionary<Action, string>> OnSetActions;
    private static Action OnRemoveActions;
    private List<GameObject> m_createdButtons = new List<GameObject>();
    #endregion

    #region MonoBehaviour
    private void OnEnable()
    {

    }

    private void Awake()
    {
        OnSetActions += CreateButtons;
        OnRemoveActions += CloseAction;
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
    public static void SetActions(Dictionary<Action, string> actions)
    {
        OnSetActions?.Invoke(actions);
    }

    public static void RemoveActions()
    {
        OnRemoveActions?.Invoke();
    }
    #endregion

    #region Private Methods
    private void CreateButtons(Dictionary<Action, string> actions)
    {
        Debug.Log($"Setting buttons {actions.Count}");
        m_root.gameObject.SetActive(true);

        foreach (var action in actions)
        {
            Action autocloseAction = action.Key;
            autocloseAction += CloseAction;
            AddButton(autocloseAction, action.Value);
        }

        AddButton(CloseAction, "Close");
    }

    #endregion

    private void AddButton(Action action, string text)
    {
        GameObject buttonGO = Instantiate(m_buttonPrefab, m_buttonsRoot);
        RadialButton button = buttonGO.GetComponent<RadialButton>();
        button.InitButton(action,text);
        m_createdButtons.Add(buttonGO);
    }

    private void CloseAction()
    {
        foreach (var button in m_createdButtons)
        {        
            Destroy(button);
        }
        m_createdButtons.Clear();
        m_root.gameObject.SetActive(false);
    }
}
