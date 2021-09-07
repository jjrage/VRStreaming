using StreamingLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatController : MonoBehaviour
{
    #region Editor variables
    [SerializeField]
    private GameObject m_messagePrefab;
    [SerializeField]
    private Transform m_chatContainer;
    #endregion

    #region Public methods
    public void AddSendedMessageToChat(MessageData messageData)
    {
        GameObject messageObj = Instantiate(m_messagePrefab, m_chatContainer);
        var messageUI = messageObj.GetComponent<MessageUI>();
        messageUI.SetMessageUI(messageData);
    }

    public void AddReceivedMessageToChat(MessageData messageData)
    {
        GameObject messageObj = Instantiate(m_messagePrefab, m_chatContainer);
        var messageUI = messageObj.GetComponent<MessageUI>();
        messageUI.SetMessageUI(messageData);
    }
    #endregion
}
