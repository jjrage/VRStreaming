using StreamingLibraryInternal;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StreamingLibrary
{
    public class Receiving
    {
        internal static StreamingBase _streamingBase;
        internal static JoinInfoBase _joinInfoBase;
        internal static ReceivingServiceSettings _receivingServiceSettings;
        private bool _isInited = false;
        private StreamingChat _streamingChat;
        private Material _capturedRenderTexture;
        private GameObject _receivingGameObject;
        private InternalEventBehaviour _eventBehaviour;
        private bool streamStarted = false;
        private bool microphoneConnected;

        /// <summary>
        /// Streaming class initializtion.
        /// </summary>
        /// <param name="streamingServiceSettings">Settings to be using for streaming.</param>
        public Receiving(ReceivingServiceSettings receivingServiceSettings)
        {
            if (receivingServiceSettings == null)
            {
                Debug.LogError("Enter valid streaming service settings.");
                return;

            }
            else
            {
                _receivingServiceSettings = receivingServiceSettings;
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
            if (_receivingGameObject == null || _eventBehaviour == null)
            {
                CreateReceivingGameObjects();
            }
            _streamingBase = new StreamingBase(_receivingServiceSettings, _receivingGameObject);
            _streamingBase.OnLeave += OnLeaveHandler;
            _streamingChat = new StreamingChat();
            //_streamingBase.OnStreamInited += StreamingBaseInited;
            _isInited = true;
        }

        private void CreateReceivingGameObjects()
        {
            _receivingGameObject = new GameObject("ReceivingInstance");
            _eventBehaviour = _receivingGameObject.AddComponent<InternalEventBehaviour>();
            _eventBehaviour.OnUpdate += Update;
            _eventBehaviour.OnFixedUpdate += FixedUpdate;
            _eventBehaviour.OnLateUpdate += LateUpdate;
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

                if (_receivingGameObject == null)
                {
                    CreateReceivingGameObjects();
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
                //_streamingBase.OnStreamInited -= StreamingBaseInited;
                //RemoveAudioSourceInputsDropdown(_receivingServiceSettings.AudioSelection);
                _streamingBase.Leave();
            }
            catch
            {
                Debug.LogError($"Error while leaving channel");
            }
        }
        private void OnLeaveHandler()
        {
            _eventBehaviour.OnUpdate -= Update;
            _eventBehaviour.OnFixedUpdate -= FixedUpdate;
            _eventBehaviour.OnLateUpdate -= LateUpdate;
            MonoBehaviour.Destroy(_eventBehaviour);
            MonoBehaviour.Destroy(_receivingGameObject);
            _joinInfoBase = null;
            streamStarted = false;
            _isInited = false;
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

        private void Update()
        {
            _streamingBase.Update();
        }

        private void FixedUpdate()
        {
            
        }

        private void LateUpdate()
        {
            //if (_joinInfoBase.IsBroadcaster)
            //{
            //    if (_capturedRenderTexture != null)
            //    {
            //        _streamingBase.LateUpdate(_capturedRenderTexture);
            //    }
            //    else
            //    {
            //        _streamingBase.LateUpdate();
            //    }
            //}
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

        //private async void AddVideoSourceInputsDropdown(Dropdown dropdown)
        //{
        //    try
        //    {
        //        var videoDevices = await _streamingBase.GetVideoSourceInput();
        //        if (videoDevices.Length > 0)
        //        {
        //            dropdown.AddOptions(videoDevices.Select(videoDevice => new Dropdown.OptionData(videoDevice.Name)).ToList());
        //            dropdown.onValueChanged.AddListener(async (i) =>
        //            {
        //                await _streamingBase._localMedia?.ChangeVideoSourceInput(videoDevices[i]);
        //            });
        //            dropdown.interactable = true;
        //        }
        //    }
        //    catch
        //    {
        //        Debug.LogError($"Error to set video source input dropdown");
        //    }
        //}

        //internal async void AddAudioSourceInputsDropdown(Dropdown dropdown)
        //{
        //    try
        //    {
        //        var audioDevices = await _streamingBase.GetAudioSourceInput();
        //        if (audioDevices.Length > 0)
        //        {
        //            dropdown.AddOptions(audioDevices.Select(audioDevice => new Dropdown.OptionData(audioDevice.Name)).ToList());
        //            dropdown.onValueChanged.AddListener(async (i) =>
        //            {
        //                await _streamingBase._localMedia?.ChangeAudioSourceInput(audioDevices[i]);
        //            });
        //            dropdown.interactable = true;
        //        }
        //    }
        //    catch
        //    {
        //        Debug.LogError($"Error to set audio source input dropdown");
        //    }
        //}

        //internal void RemoveAudioSourceInputsDropdown(Dropdown dropdown)
        //{
        //    dropdown.ClearOptions();
        //}
    }
}
