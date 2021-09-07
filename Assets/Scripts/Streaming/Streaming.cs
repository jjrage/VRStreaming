using FM.LiveSwitch;
using FM.LiveSwitch.Unity;
using StreamingLibraryInternal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace StreamingLibrary
{
    public class Streaming
    {
        internal static Queue<MessageData> receivedMessagesFromDataChannelsQueue = new Queue<MessageData>();
        internal static Queue<MessageData> receivedMessagesFromMainChannel = new Queue<MessageData>();
        internal static Queue<MessageData> sendedMessagesToDataChannelsQueue = new Queue<MessageData>();
        internal static Queue<MessageData> sendedMessagesToMainChannelQueue = new Queue<MessageData>();

        /// <summary>
        /// Callback that is invoked every time the user send a message.
        /// </summary>
        private Action<MessageData> OnMessageSended;
        /// <summary>
        /// Callback that is invoked every time the streaming channel got a message.
        /// </summary>
        public Action<MessageData> OnMessageReceived;

        internal static StreamingBase _streamingBase;
        internal static JoinInfoBase _joinInfoBase;
        internal static StreamingServiceSettings _streamingServiceSettings;
        private bool _isInited = false;
        private StreamingChat _streamingChat;
        private RenderTexture _capturedRenderTexture;
        private GameObject _streamingGameObject;
        private InternalEventBehaviour _eventBehaviour;
        private bool streamStarted = false;
        private bool microphoneConnected;

        /// <summary>
        /// Streaming class initializtion.
        /// </summary>
        /// <param name="streamingServiceSettings">Settings to be using for streaming.</param>
        public Streaming(StreamingServiceSettings streamingServiceSettings)
        {
            if (streamingServiceSettings != null)
            {
                _streamingServiceSettings = streamingServiceSettings;
                _capturedRenderTexture = streamingServiceSettings.RenderTexture;
            }
            else
            {
                Debug.LogError("Enter valid streaming service settings.");
                return;
            }
        }

        /// <summary>
        /// Dispose internal data and instances.
        /// </summary>
        public void Dispose()
        {
            _isInited = false;
            _streamingBase = null;
            _streamingChat = null;
            _capturedRenderTexture = null;
        }

        private void Init()
        {
            if (_streamingGameObject == null || _eventBehaviour == null)
            {
                CreateStreamingGameObjects();
            }
            _streamingBase = new StreamingBase(_streamingServiceSettings, _streamingGameObject);
            _streamingChat = new StreamingChat();
            _streamingBase.OnStreamInited += StreamingBaseInited;
            _isInited = true;
        }

        private void StreamingBaseInited()
        {
            if(_streamingServiceSettings != null && _streamingServiceSettings.AudioSelection != null)
            {
                AddAudioSourceInputsDropdown(_streamingServiceSettings.AudioSelection);
            }
        }

        private void CreateStreamingGameObjects()
        {
            _streamingGameObject = new GameObject("StreamingInstance");
            _eventBehaviour = _streamingGameObject.AddComponent<InternalEventBehaviour>();
            _eventBehaviour.OnUpdate += Update;
            _eventBehaviour.OnFixedUpdate += FixedUpdate;
            _eventBehaviour.OnLateUpdate += LateUpdate;
        }

        /// <summary>
        /// Start streaming.
        /// </summary>
        /// <param name="userName">Name of broadcaster user.</param>
        /// <param name="channelName">Name of channel to connect.</param>
        public void StartStreaming(string userName, string channelName) //TODO: checker for device type and client type as well
        {
            microphoneConnected = Microphone.devices.Length > 0;
            if (microphoneConnected)
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(channelName) && !streamStarted)
                {
                    if (!_isInited)
                    {
                        Init();
                    }
                    else
                    {
                        Debug.LogError("Streaming service not inited yet, please init service with correct settings and try again!");
                    }

                    if (_streamingGameObject == null)
                    {
                        CreateStreamingGameObjects();
                    }
                    _joinInfoBase = new JoinInfoBase(userName, channelName, true);
                    _streamingBase.StartStream(_joinInfoBase);
                    streamStarted = true;
                }
                else
                {
                    Debug.LogError("Enter valid username and channel name.");
                }
            }
            else
            {
                Debug.LogError("There are no any input device found, please connect input device and try again");
            }

        }
        /// <summary>
        /// Join to existing channel.
        /// </summary>
        /// <param name="userName">Name of broadcaster user.</param>
        /// <param name="channelName">Name of channel to connect.</param>
        public void JoinChannel(string userName, string channelName) //TODO: checker for device type and client type as well
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(channelName) && !streamStarted)
            {
                if (!_isInited)
                {
                    Init();
                }
                else
                {
                    Debug.LogError("Streaming service not inited yet, please init service with correct settings and try again!");
                }

                if (_streamingGameObject == null)
                {
                    CreateStreamingGameObjects();
                }

                _joinInfoBase = new JoinInfoBase(userName, channelName, false);
                _streamingBase.StartStream(_joinInfoBase);
                streamStarted = true;
            }
            else
            {
                Debug.LogError("Enter valid username and channel name.");
            }
        }

        /// <summary>
        /// Leave streaming channel.
        /// </summary>
        public void Leave()
        {
            try
            {
                _streamingBase.OnStreamInited -= StreamingBaseInited;
                RemoveAudioSourceInputsDropdown(_streamingServiceSettings.AudioSelection);
                _streamingBase.Leave();
                _eventBehaviour.OnUpdate -= Update;
                _eventBehaviour.OnFixedUpdate -= FixedUpdate;
                _eventBehaviour.OnLateUpdate -= LateUpdate;
                MonoBehaviour.Destroy(_eventBehaviour);
                MonoBehaviour.Destroy(_streamingGameObject);
                _joinInfoBase = null;
                streamStarted = false;
                _isInited = false;
            }
            catch
            {
                Debug.LogError($"Error while leaving channel");
            }
        }

        internal void ChatUpdate()
        {
            if (receivedMessagesFromMainChannel.Count > 0)
            {
                var messageData = receivedMessagesFromMainChannel.Dequeue();
                OnMessageReceived?.Invoke(messageData);
            }

            if (sendedMessagesToMainChannelQueue.Count > 0)
            {
                var messageData = sendedMessagesToMainChannelQueue.Dequeue();
                OnMessageSended?.Invoke(messageData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        internal void SendDataChannelMessage(string message)
        {
            if (_streamingBase != null)
            {
                _streamingBase.SendDataChannelMessage(message);
            }
        }

        internal void OnMessageSendedHandler(string message)
        {
            MessageData messageData = new MessageData
            {
                Sender = _joinInfoBase.Name,
                TextMessage = message
            };

            OnMessageSended?.Invoke(messageData);
        }

        internal void OnMessageReceivedHandler(string message)
        {
            MessageData messageData = new MessageData
            {
                Sender = _joinInfoBase.Name,
                TextMessage = message
            };

            OnMessageReceived?.Invoke(messageData);
        }

        private void Update()
        {
            _streamingBase.Update();
        }

        private void FixedUpdate()
        {
            ChatUpdate();
        }

        private void LateUpdate()
        {
            if (_joinInfoBase.IsBroadcaster)
            {
                if (_capturedRenderTexture != null)
                {
                    _streamingBase.LateUpdate(_capturedRenderTexture);
                }
                else
                {
                    _streamingBase.LateUpdate();
                }
            }
        }

        /// <summary>
        /// Processing and sending video data that present in render texture to remote user. Must be called in Unity's LateUpdate() method.
        /// </summary>
        /// <param name="renderTexture"></param>
        private void ProcessVideoData(RenderTexture renderTexture)
        {
            if (_joinInfoBase.IsBroadcaster)
            {
                _streamingBase.LateUpdate(renderTexture);
            }
        }

        /// <summary>
        /// Processing and sending video data captured by local camera to remote user. Must be called in Unity's LateUpdate() method.
        /// </summary>
        private void ProcessVideoData()
        {
            if (_joinInfoBase.IsBroadcaster)
            {
                _streamingBase.LateUpdate();
            }
        }

        private async void AddVideoSourceInputsDropdown(Dropdown dropdown)
        {
            try
            {
                var videoDevices = await _streamingBase.GetVideoSourceInput();
                if (videoDevices.Length > 0)
                {
                    dropdown.AddOptions(videoDevices.Select(videoDevice => new Dropdown.OptionData(videoDevice.Name)).ToList());
                    dropdown.onValueChanged.AddListener(async (i) =>
                    {
                        await _streamingBase._localMedia?.ChangeVideoSourceInput(videoDevices[i]);
                    });
                    dropdown.interactable = true;
                }
            }
            catch
            {
                Debug.LogError($"Error to set video source input dropdown");
            }
        }

        internal async void AddAudioSourceInputsDropdown(Dropdown dropdown)
        {
            try
            {
                var audioDevices = await _streamingBase.GetAudioSourceInput();
                if (audioDevices.Length > 0)
                {
                    dropdown.AddOptions(audioDevices.Select(audioDevice => new Dropdown.OptionData(audioDevice.Name)).ToList());
                    dropdown.onValueChanged.AddListener(async (i) =>
                    {
                        await _streamingBase._localMedia?.ChangeAudioSourceInput(audioDevices[i]);
                    });
                    dropdown.interactable = true;
                }
            }
            catch
            {
                Debug.LogError($"Error to set audio source input dropdown");
            }
        }

        internal void RemoveAudioSourceInputsDropdown(Dropdown dropdown)
        {
            dropdown.ClearOptions();
        }
    }
}
