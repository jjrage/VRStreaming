using StreamingLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReceivingTest : MonoBehaviour
{
    public delegate void OnEntrancePasswordSubmited(string password);
    public static OnEntrancePasswordSubmited entrancePasswordSubmited;

    #region Editor variables
    [SerializeField]
    private TMP_InputField _userNameInput;
    [SerializeField]
    private TMP_InputField _channelNameInput;
    [SerializeField]
    private Button _startStreamButton;
    [SerializeField]
    private Button _leaveStreamButton;
    [SerializeField]
    private Material _materialForDisplay;
    #endregion

    #region Private variables
    private Receiving _receiving;
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
        entrancePasswordSubmited += DisplayPassword;
        //_startStreamButton.onClick.AddListener(JoinStream);
        //_leaveStreamButton.onClick.AddListener(Leave);
        //_userNameInput.text = "UnityClient";
        //_channelNameInput.text = "123";
    }

    private void DisplayPassword(string password)
    {
        JoinStream("VRStreaming", password);
        Debug.Log(password);
    }

    private void OnDisable()
    {
        if (_receiving != null)
        {
            Leave();
            _receiving.Dispose();
        }
    }
    #endregion

    private void JoinStream()
    {
        if (!string.IsNullOrEmpty(_userNameInput.text) && !string.IsNullOrEmpty(_channelNameInput.text))
        {
            StreamingLibrary.AudioSettings audioSettings = new StreamingLibrary.AudioSettings();
            ReceivingServiceSettings receivingServiceSettings = new ReceivingServiceSettings(_materialForDisplay, audioSettings);
            receivingServiceSettings.ReceiveOwnMessages = true;
            if (_receiving == null)
            {
                _receiving = new Receiving(receivingServiceSettings); //Streaming service initialization with prefered settings
            }

            //Callback that is invoked every time the streaming channel got a message
            //Method for starting stream. Please, start stream only with valid username and channel name
            _receiving.JoinChannel(_userNameInput.text, _channelNameInput.text);
        }
    }
    private void JoinStream(string channel, string name)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(channel))
        {
            StreamingLibrary.AudioSettings audioSettings = new StreamingLibrary.AudioSettings();
            ReceivingServiceSettings receivingServiceSettings = new ReceivingServiceSettings(_materialForDisplay, audioSettings);
            receivingServiceSettings.ReceiveOwnMessages = true;
            if (_receiving == null)
            {
                _receiving = new Receiving(receivingServiceSettings); //Streaming service initialization with prefered settings
            }

            //Method for starting stream. Please, start stream only with valid username and channel name
            _receiving.JoinChannel(name, channel);
        }
    }

    //Leave streaming channel. Please, leave streaming channel before exit playmode
    private void Leave()
    {
        _receiving.Leave();
    }
    #endregion

    public void SubmitName(string name)
    {
        
        Debug.Log(name);
    }
}
