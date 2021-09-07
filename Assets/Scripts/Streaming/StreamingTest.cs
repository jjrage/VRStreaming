using StreamingLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class StreamingTest : MonoBehaviour
{
    #region Editor variables
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private InputField _chatInput;
    [SerializeField]
    private InputField _userNameInput;
    [SerializeField]
    private InputField _channelNameInput;
    [SerializeField]
    private ChatController _chatController;
    [SerializeField]
    private RenderTexture _renderTexture;
    [SerializeField]
    private Button _startStreamButton;
    [SerializeField]
    private Button _leaveStreamButton;
    [SerializeField]
    private Button _sendMessageButton;
    [SerializeField]
    private Dropdown _audioSelect;
    [SerializeField]
    private RawImage _localImage;
    #endregion

    #region Private variables
    private Streaming _streaming;
    private int _videoHeight = 0; //Height of captured data. For now FullHD is recomended for perfomance reasons.
    private int _videoWidth = 0; //Width of captured data. For now FullHD is recomended for perfomance reasons.
    private int _videoBitrate = 6000; //Prefered Bitrate value. This approximate value and it depends on general perfomance.
    private int _videoFPS = 30; //Prefered FPS value. This approximate value and it depends on general perfomance.
    private int _audioBitrate = 3000; //Prefered Audio bitrate value. This approximate value and it depends on general perfomance.
    #endregion

    #region Private merhods

    #region Monobehaviour methods
    private void OnEnable()
    {
        _localImage.texture = _renderTexture;
        _startStreamButton.onClick.AddListener(StartStream);
        _leaveStreamButton.onClick.AddListener(Leave);
    }

    private void OnDisable()
    {
        if(_streaming != null)
        {
            Leave();
            _streaming.Dispose();
        }
    }
    #endregion

    private void StartStream()
    {
        if(!string.IsNullOrEmpty(_userNameInput.text) && !string.IsNullOrEmpty(_channelNameInput.text))
        {
            StreamingServiceSettings streamingServiceSettings = new StreamingServiceSettings(); //Settings for streaming service.
            streamingServiceSettings.LocalCamera = _camera; //Camera from local scene that capturing video data into render texture.
            streamingServiceSettings.RenderTexture = _renderTexture; //Render texture with captured data. In case if render texture won't be setted streaming service will use render texture assigned to local camera.
            streamingServiceSettings.AudioSelection = _audioSelect;
            streamingServiceSettings.ReceiveOwnMessages = false;
            _videoHeight = _renderTexture.height;
            _videoWidth = _renderTexture.width;
            VideoSettings videoSettings = new VideoSettings(_videoHeight, _videoWidth, _videoFPS, _videoBitrate); //Prefered video settings.
            StreamingLibrary.AudioSettings audioSettings = new StreamingLibrary.AudioSettings(_audioBitrate); //Prefered aduio settings.
            streamingServiceSettings.VideoSettings = videoSettings;
            streamingServiceSettings.AudioSettings = audioSettings;

            if (_streaming == null)
            {
                _streaming = new Streaming(streamingServiceSettings); //Streaming service initialization with prefered settings
            }

            //Callback that is invoked every time the streaming channel got a message
            _streaming.OnMessageReceived += _chatController.AddReceivedMessageToChat;
            //Method for starting stream. Please, start stream only with valid username and channel name
            _streaming.StartStreaming(_userNameInput.text, _channelNameInput.text);
        }
    }

    //Leave streaming channel. Please, leave streaming channel before exit playmode
    private void Leave()
    {
        _streaming.Leave();
        _streaming.OnMessageReceived -= _chatController.AddReceivedMessageToChat;
        //_streaming.OnMessageSended -= _chatController.AddSendedMessageToChat;
    }
    #endregion
}
