using StreamingLibraryInternal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StreamingLibrary
{
    public class StreamingChat
    {
        //internal static Queue<MessageData> receivedMessagesFromDataChannelsQueue = new Queue<MessageData>();
        //internal static Queue<MessageData> receivedMessagesFromMainChannel = new Queue<MessageData>();
        //internal static Queue<MessageData> sendedMessagesToDataChannelsQueue = new Queue<MessageData>();
        //internal static Queue<MessageData> sendedMessagesToMainChannelQueue = new Queue<MessageData>();


        //public static StreamingChat instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new StreamingChat();
        //        }
        //        return _instance;
        //    }
        //}



        //public Action<MessageData> OnMessageSended;
        //public Action<MessageData> OnMessageReceived;

        //private static StreamingChat _instance;
        //private object messageLock = new object();
        //private Thread receivingTextThread;
        //private bool messagesHandlingStarted = false;

        //internal void ChatUpdate()
        //{
        //    if (receivedMessagesFromMainChannel.Count > 0)
        //    {
        //        var messageData = receivedMessagesFromMainChannel.Dequeue();
        //        OnMessageReceived?.Invoke(messageData);
        //    }

        //    if (sendedMessagesToMainChannelQueue.Count > 0)
        //    {
        //        var messageData = sendedMessagesToMainChannelQueue.Dequeue();
        //        OnMessageSended?.Invoke(messageData);
        //    }

        //}

        //public void SendDataChannelMessage(string message)
        //{

        //    if (Streaming._streamingBase != null)
        //    {
        //        Streaming._streamingBase.SendDataChannelMessage(message);
        //    }
        //}
        //public void SendMainChannelMessage(string message)
        //{

        //    if (Streaming._streamingBase != null)
        //    {
        //        Streaming._streamingBase.SendMainChannelMessage(message);
        //    }
        //}

        //internal void OnMessageSendedHandler(string message)
        //{
        //    MessageData messageData = new MessageData
        //    {
        //        Sender = Streaming._joinInfoBase.Name,
        //        TextMessage = message
        //    };

        //    OnMessageSended?.Invoke(messageData);
        //}

        //internal void OnMessageReceivedHandler(string message)
        //{
        //    MessageData messageData = new MessageData
        //    {
        //        Sender = Streaming._joinInfoBase.Name,
        //        TextMessage = message
        //    };

        //    OnMessageReceived?.Invoke(messageData);
        //}

        //internal void Init()
        //{
        //    //StreamingBase.Streamingbase.OnTextMessageSended += OnMessageSendedHandler;
        //    //StreamingBase.Streamingbase.OnMessageReceived += OnMessageReceivedHandler;
        //}
    }
}
