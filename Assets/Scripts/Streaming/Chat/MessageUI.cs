using StreamingLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    #region Editor variables
    [SerializeField]
    private Text userName;
    [SerializeField]
    private Text messageData;
    #endregion

    #region Public methods
    public void SetMessageUI(MessageData messageData)
    {
        userName.text = messageData.Sender;
        this.messageData.text = messageData.TextMessage;
    }
    #endregion
}
