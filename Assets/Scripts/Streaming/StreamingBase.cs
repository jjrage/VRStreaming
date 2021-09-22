using FM.LiveSwitch;
using FM.LiveSwitch.Unity;
using StreamingLibrary;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace StreamingLibraryInternal
{
    internal class StreamingBase
    {
        internal StreamingBase Streamingbase
        {
            get
            {
                if (_instance == null)
                {
                    //_instance = new StreamingBase();
                }

                return _instance;
            }
        }

        #region Internal fields
        internal LocalMedia _localMedia;
        internal RemoteMedia _remoteMedia;
        #region Callbacks
        internal Action OnStreamInited;
        internal Action<string> OnMessageReceived;
        internal Action<string> OnTextMessageSended;
        internal Action<DataBuffer> OnBytesMessageSended;
        internal Action OnLeave;
        #endregion

        #endregion

        #region Private fields

        #region Const
        private const int INITIAL_REGISTER_BACKOFF = 200; // milliseconds
        private const int MAX_REGISTER_BACKOFF = 60000; // milliseconds
        #endregion

        #region Readonly
        private readonly string _UserId = Guid.NewGuid().ToString().Replace("-", "");
        private readonly string _DeviceId = Guid.NewGuid().ToString().Replace("-", "");
        #endregion

        private ILog _log = Log.GetLogger(typeof(StreamingBase));
        private int _registerBackoff = INITIAL_REGISTER_BACKOFF;
        private Client _client;
        private Channel _channel;
        private SfuUpstreamConnection _sfuUpstreamConnection;
        private Dictionary<string, SfuDownstreamConnection> _SfuDownStreamConnections;
        private Dictionary<string, bool> _localAVMaps;
        private List<bool> _sendEncodings;
        private List<DataChannel> _dataChannels = new List<DataChannel>();
        private Camera localCamera;
        private AecContext _aecContext;
        private Material _materialToDisplay;
        private Texture _textureToDisplay;
        private MonoBehaviour _behaviourProxy;
        private Texture2D screenShot;
        private StreamingBase _instance;
        private JoinInfoBase _currentJoinInfo;
        private VideoSettings videoSettings;
        private StreamingLibrary.AudioSettings audioSettings;
        private ConcurrentQueue<Func<Task>> _mainThreadQueue = new ConcurrentQueue<Func<Task>>();
        private Mode _mode;
        private bool _isBroadcaster;
        private bool _disableAudio = false;
        private bool _disableVideo = false;
        private bool _unregistering = false;
        private bool needLoger = false;
        private bool _audioOnly;
        private bool _receiveOnly;
        private bool _captureWithUnityCamera = true;
        private string _name;
        private string _channelId;
        private string _materialName;
        private object _dataChannelLock = new object();
        private bool _receiveOwnMessages = false;
        #endregion

        #region Public methods

        #endregion

        #region Private methods

        #endregion

        #region Internal methods
        internal StreamingBase(StreamingServiceSettings streamingServiceSettings, GameObject streamingGO)
        {
            _behaviourProxy = streamingGO.GetComponent<MonoBehaviour>();
            localCamera = streamingServiceSettings.LocalCamera;

            if (streamingServiceSettings.VideoSettings != null)
            {
                videoSettings = streamingServiceSettings.VideoSettings;
            }
            else
            {
                videoSettings = new VideoSettings();
            }

            if (streamingServiceSettings.AudioSettings != null)
            {
                audioSettings = streamingServiceSettings.AudioSettings;
            }
            else
            {
                audioSettings = new StreamingLibrary.AudioSettings();
            }

            _receiveOwnMessages = streamingServiceSettings.ReceiveOwnMessages;
        }

        internal StreamingBase(ReceivingServiceSettings receivingServiceSettings, GameObject streamingGO)
        {
            _behaviourProxy = streamingGO.GetComponent<MonoBehaviour>();
            _materialName = receivingServiceSettings.MaterialName;
            _receiveOwnMessages = receivingServiceSettings.ReceiveOwnMessages;

            if (receivingServiceSettings.AudioSettings != null)
            {
                audioSettings = receivingServiceSettings.AudioSettings;
            }

            if (receivingServiceSettings.MaterialForDisplay != null)
            {
                _materialToDisplay = receivingServiceSettings.MaterialForDisplay;

            }
            else
            {
                Debug.LogError("Please, set correct material for displaying remote media");
                return;
            }

        }

        internal void StartStream(JoinInfoBase joinInfoBase)
        {
            _currentJoinInfo = joinInfoBase;
            _behaviourProxy.StartCoroutine(Start(_currentJoinInfo));
        }

        internal async void Update()
        {
            var count = _mainThreadQueue.Count;
            for (var i = 0; i < count; i++)
            {
                if (_mainThreadQueue.TryDequeue(out var action))
                {
                    await action();
                }
            }
        }

        internal void LateUpdate()
        {
            _behaviourProxy.StartCoroutine(CaptureFromUnityCamera());
        }

        internal void LateUpdate(RenderTexture renderTexture)
        {
            //_behaviourProxy.StartCoroutine(CaptureFromRenderTexture(renderTexture));
            _behaviourProxy.StartCoroutine(CaptureFromRenderTextureGPU(renderTexture));
        }
        #endregion

        #region Coroutines

        internal IEnumerator Start(JoinInfoBase joinInfoBase)
        {
            if (needLoger)
            {
                Log.RegisterProvider(new FM.LiveSwitch.Unity.DebugLogProvider(LogLevel.Debug));
            }

            //Log.RegisterProvider(new FM.LiveSwitch.Unity.TextLogProvider(txtLog, LogLevel.Info));

            _currentJoinInfo = joinInfoBase;
            _name = _currentJoinInfo.Name;
            _channelId = _currentJoinInfo.ChannelId;
            _audioOnly = _currentJoinInfo.AudioOnly;
            _receiveOnly = _currentJoinInfo.ReceiveOnly;
            _captureWithUnityCamera = _currentJoinInfo.CaptureWithUnityCamera;
            _mode = _currentJoinInfo.Mode;
            _isBroadcaster = _currentJoinInfo.IsBroadcaster;
            _localAVMaps = new Dictionary<string, bool>();
            _sendEncodings = new List<bool>();
            _SfuDownStreamConnections = new Dictionary<string, SfuDownstreamConnection>();
            yield return Join().AsIEnumerator();
        }

        internal IEnumerator CaptureFromUnityCamera()
        {
            yield return new WaitForEndOfFrame();

            if (_captureWithUnityCamera)
            {
                var localTexture2DMedia = _localMedia as CustomLocalTexture2DMedia;

                if (localTexture2DMedia != null)
                {
                    SendImageFromLocalCamera(localTexture2DMedia);
                }
            }
        }

        internal IEnumerator CaptureFromRenderTexture(RenderTexture renderTexture)
        {
            yield return new WaitForEndOfFrame();

            if (_captureWithUnityCamera)
            {
                var localTexture2DMedia = _localMedia as CustomLocalTexture2DMedia;

                if (localTexture2DMedia != null)
                {
                    try
                    {
                        SendImageFromRenderTexture(localTexture2DMedia, renderTexture);
                    }
                    catch
                    {

                    }
                }
            }
        }

        internal IEnumerator CaptureFromUnityCameraGPU(RenderTexture renderTexture)
        {
            yield return new WaitForEndOfFrame();

            if (_captureWithUnityCamera)
            {
                var localTexture2DMedia = _localMedia as CustomLocalTexture2DMedia;

                if (localTexture2DMedia != null)
                {
                    AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.ARGB32, OnCompleteReadback);

                }
            }
        }

        internal IEnumerator CaptureFromRenderTextureGPU(RenderTexture renderTexture)
        {
            yield return new WaitForEndOfFrame();

            AsyncGPUReadback.Request(renderTexture, 0, renderTexture.graphicsFormat, OnCompleteReadback);
        }

        #endregion

        internal async Task Join()
        {
            try
            {
                _client = new Client(_currentJoinInfo.Gateway, _currentJoinInfo.AppId, _UserId, _DeviceId)
                {
                    Tag = _mode.ToString(),
                    DeviceAlias = "a-device-alias",
                    UserAlias = _name
                };

                // log client state
                _client.OnStateChange += async (client) =>
                {
                    if (needLoger)
                    {
                        //Debug.Log("can log!");
                        _log.Info(client.Id, $"Client is {client.State.ToString().ToLower()}.");
                    }

                    if (client.State == ClientState.Unregistered && !_unregistering)
                    {
                        // apply retry backoff
                        await Task.Delay(_registerBackoff);

                        // increase register backoff
                        _registerBackoff = Math.Min(_registerBackoff * 2, MAX_REGISTER_BACKOFF);

                        // re-register/join
                        _mainThreadQueue.Enqueue(async () =>
                        {
                            try
                            {
                                await Register();

                                // reset backoff
                                _registerBackoff = INITIAL_REGISTER_BACKOFF;
                            }
                            catch
                            {
                                if (needLoger)
                                {
                                    _log.Error("Failed to re-register/join.");
                                }
                            }
                        });
                    }
                };

                // register/join
                await Register();
            }
            catch
            {
                Debug.LogError("Error while joining to channel");
            }
        }

        private async Task Register()
        {
            try
            {
                var registerToken = Token.GenerateClientRegisterToken(_client, _channelId, _currentJoinInfo.SharedKey);

                // register (and join at the same time)
                var channels = await _client.Register(registerToken);
                // get the channel reference
                _channel = channels.First();
                // log channel events
                _channel.OnRemoteClientJoin += (remoteClientInfo) =>
                {
                    if (needLoger)
                    {
                        _log.Error($"Remote client joined: {remoteClientInfo.ToJson()}");
                    }
                };
                _channel.OnRemoteClientUpdate += (remoteClientInfo, newRemoteClientInfo) =>
                {
                    if (needLoger)
                    {
                        _log.Info(remoteClientInfo.Id, $"Remote client updated: {newRemoteClientInfo.ToJson()}");
                    }
                };
                _channel.OnRemoteClientLeave += (remoteClientInfo) =>
                {
                    if (needLoger)
                    {
                        _log.Info(remoteClientInfo.Id, $"Remote client left: {remoteClientInfo.ToJson()}");
                    }
                };
                _channel.OnRemoteUpstreamConnectionOpen += (remoteConnectionInfo) =>
                {
                    if (needLoger)
                    {
                        _log.Info(remoteConnectionInfo.Id, $"Remote upstream connection opened: {remoteConnectionInfo.ToJson()}");
                    }
                };
                _channel.OnRemoteUpstreamConnectionUpdate += (remoteConnectionInfo, newRemoteConnectionInfo) =>
                {
                    if (needLoger)
                    {
                        _log.Info(remoteConnectionInfo.Id, $"Remote upstream connection updated: {newRemoteConnectionInfo.ToJson()}");
                    }
                };
                _channel.OnRemoteUpstreamConnectionClose += (remoteConnectionInfo) =>
                {
                    if (needLoger)
                    {
                        _log.Info(remoteConnectionInfo.Id, $"Remote upstream connection closed: {remoteConnectionInfo.ToJson()}");
                    }
                };

                _channel.OnMessage += (clientInfo, message) =>
                {
                    if (!_receiveOwnMessages && _name == clientInfo.UserAlias)
                    {
                        return;
                    }

                    if (_isBroadcaster)
                    {
                        MessageData chatMessage = new MessageData();
                        chatMessage.Sender = clientInfo.UserAlias;
                        chatMessage.TextMessage = message;
                    }
                    else
                    {
                        MessageData chatMessage = new MessageData();
                        chatMessage.Sender = clientInfo.UserAlias;
                        chatMessage.TextMessage = message;
                    }
                };
#if !UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX && !UNITY_IOS
                // create AEC context (optional)
                //_aecContext = new AecContext();
#endif
                if (_isBroadcaster)
                {
                    var customMedia = new CustomLocalTexture2DMedia(disableAudio: _disableAudio, disableVideo: _disableVideo);
                    customMedia.SetVideoSettings(new Size(videoSettings.Width, videoSettings.Height), videoSettings.FPS);
                    customMedia.SetAudioSettings(audioSettings);
                    customMedia.Initialize();
                    customMedia.OnActiveVideoEncoderChange += ActiveVideoEncoderChange;
                    _localMedia = customMedia;
                    // start local media
                    await _localMedia.Start();
                }

                _channel.OnRemoteUpstreamConnectionOpen += (remoteConnectionInfo) =>
                {
                    // open SFU downstream connection
                    _mainThreadQueue.Enqueue(() => OpenSfuDownstreamConnection(remoteConnectionInfo));
                };

                foreach (var remoteConnectionInfo in _channel.RemoteUpstreamConnectionInfos)
                {
                    // open SFU downstream connection
                    _mainThreadQueue.Enqueue(() => OpenSfuDownstreamConnection(remoteConnectionInfo));
                }

                if (_mode == Mode.Sfu)
                {
                    if (!_receiveOnly)
                    {
                        // open SFU upstream connection
                        _mainThreadQueue.Enqueue(OpenSfuUpstreamConnection);
                    }
                }
                OnStreamInited?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while register client: {ex.Message}");
            }
        }

        private void ActiveAudioPacketizerChange(AudioPipe p)
        {
            if (p.State == MediaPipeState.Initialized)
            {
                if (videoSettings != null)
                {
                    //p.bitrate
                }
                else
                {
                    //Debug.LogError("Can't set bitrate since video settings are not setted yet");
                }
            }
        }

        private void ActiveVideoEncoderChange(VideoEncoder p)
        {
            if (p.State == MediaPipeState.Initialized)
            {
                if (videoSettings != null)
                {
                    p.TargetBitrate = videoSettings.Bitrate;
                    p.StaticOutputBitrate = true;
                    p.MaxBitrate = 6000;
                    int minBitrate = videoSettings.Bitrate - 1000;

                    if (minBitrate < 1024)
                    {
                        minBitrate = 1024;
                    }

                    p.MinBitrate = minBitrate;
                }
                else
                {
                    Debug.LogError("Can't set bitrate since video settings are not setted yet");
                }
            }
        }

        internal async Task<SourceInput[]> GetVideoSourceInput()
        {
            var videoDevices = await _localMedia.GetVideoSourceInputs();
            return videoDevices;
        }

        internal async Task<SourceInput[]> GetAudioSourceInput()
        {
            var audioDevices = await _localMedia.GetAudioSourceInputs();
            return audioDevices;
        }

        internal async void Leave()
        {
            try
            {
                if (_localMedia != null)
                {
                    await _localMedia.Stop();
                    _localMedia.Destroy();
                    _localMedia = null;
                }
                //if(_remoteMedia != null)
                //{
                //    _remoteMedia.Destroy();
                //    _remoteMedia = null;
                //}
                if (_client != null)
                {
                    await _client.Unregister();
                    _client = null;
                }

                var eventBehaviours = GameObject.FindObjectsOfType<EventBehaviour>();
                var internalEventBehaviours = GameObject.FindObjectsOfType<InternalEventBehaviour>();

                foreach (var eventBehaviour in eventBehaviours)
                {
                    MonoBehaviour.Destroy(eventBehaviour.gameObject);
                }

                foreach (var internalEventBehaviour in internalEventBehaviours)
                {
                    MonoBehaviour.Destroy(internalEventBehaviour.gameObject);
                }

                if (!_isBroadcaster)
                {
                    _materialToDisplay.mainTexture = null;
                }
                OnLeave?.Invoke();
            }
            catch
            {
                Debug.LogError("Error while leaving from channel");
            }
        }

        private async Task OpenSfuUpstreamConnection()
        {
            try
            {
                AudioStream audioStream = null;
                VideoStream videoStream = null;
                DataChannel dataChannel = PrepareDataChannel();
                DataStream dataStream = new DataStream(dataChannel);
                SfuUpstreamConnection sfuUpstreamConnection = null;

                lock (_dataChannelLock)
                {
                    _dataChannels.Add(dataChannel);
                }

                _dataChannels.Add(dataChannel);

                if (_isBroadcaster)
                {
                    //Debug.Log("Upstream connection created as broadcaster");
                    audioStream = new AudioStream(_localMedia, remoteMedia: null);
                    videoStream = _audioOnly ? null : new VideoStream(_localMedia);

                    sfuUpstreamConnection = _channel?.CreateSfuUpstreamConnection(audioStream, videoStream, dataStream);
                }
                else
                {
                    sfuUpstreamConnection = _channel?.CreateSfuUpstreamConnection(dataStream);
                }

                _sfuUpstreamConnection = sfuUpstreamConnection;
                // log connection state
                sfuUpstreamConnection.OnStateChange += (connection) =>
                {
                    if (needLoger)
                    {
                        _log.Info(connection.Id, $"SFU upstream connection is {connection.State.ToString().ToLower()}.");
                    }

                    if (connection.State == ConnectionState.Closing || connection.State == ConnectionState.Failing)
                    {
                        if (connection.RemoteClosed)
                        {
                            if (needLoger)
                            {
                                _log.Info("{0}: Media server closed the connection.", connection.Id);
                            }
                        }
                        _sfuUpstreamConnection = null;

                        lock (_dataChannelLock)
                        {
                            _dataChannels.Remove(dataChannel);
                        }

                        _dataChannels.Remove(dataChannel);

                    }
                    // retry on failure
                    if (connection.State == ConnectionState.Failed)
                    {
                        _mainThreadQueue?.Enqueue(OpenSfuUpstreamConnection);
                    }
                };

                // open connection
                await sfuUpstreamConnection.Open();
            }
            catch
            {
                Debug.LogError("Error while opening SFU upstream connection");
            }
        }
        private async Task OpenSfuDownstreamConnection(ConnectionInfo remoteConnectionInfo)
        {
            try
            {
                AudioStream audioStream = null;
                VideoStream videoStream = null;
                DataChannel dataChannel = null;
                DataStream dataStream = null;
                RemoteMedia remoteMedia = null;
                SfuDownstreamConnection sfuDownstreamConnection = null;

                if (remoteConnectionInfo.HasData)
                {
                    dataChannel = PrepareDataChannel();
                    dataStream = new DataStream(dataChannel);
                }

                if (_audioOnly && !remoteConnectionInfo.HasAudio)
                {
                    // not a match
                    return;
                }

                //_aecContext.SetAudioSettings(audioSettings);

                if (_isBroadcaster)
                {
                    //Debug.Log("creating remote media as broadcaster");
                    // create remote media
                    //remoteMedia = new RemoteMedia(disableAudio: true, disableVideo: true, _aecContext);
                    //remoteMedia.Initialize();
                    sfuDownstreamConnection = _channel?.CreateSfuDownstreamConnection(remoteConnectionInfo, dataStream);
                }
                else
                {
                    _aecContext = new AecContext();
                    //Debug.Log("creating remote media as audience");
                    // create remote media
                    remoteMedia = new RemoteMedia(disableAudio: false, disableVideo: false, _aecContext);
                    remoteMedia.SetAudioSettings(audioSettings);
                    remoteMedia.Initialize();
                    // create streams
                    audioStream = new AudioStream(localMedia: null, remoteMedia);
                    videoStream = _audioOnly ? null : new VideoStream(localMedia: null, remoteMedia);
                    _materialToDisplay.SetTexture(_materialName, remoteMedia.View.GetComponentInChildren<RawImage>().texture);
                    sfuDownstreamConnection = _channel?.CreateSfuDownstreamConnection(remoteConnectionInfo, audioStream, videoStream, dataStream);
                }

                // create connection
                //_SfuDownStreamConnections.Add(remoteMedia.Id, sfuDownstreamConnection);

                // log connection state
                sfuDownstreamConnection.OnStateChange += (connection) =>
                {
                    if (needLoger)
                    {
                        _log.Info(connection.Id, $"SFU downstream connection is {connection.State.ToString().ToLower()}. ({remoteConnectionInfo.ToJson()})");
                    }

                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Failed)
                    {
                        // _SfuDownStreamConnections.Remove(remoteMedia.Id);
                        _mainThreadQueue.Enqueue(() =>
                        {
                            if (remoteMedia != null)
                            {
                                remoteMedia.Destroy();
                            }

                            return Task.CompletedTask;
                        });
                    }
                    // retry on failure
                    if (connection.State == ConnectionState.Failed)
                    {
                        _mainThreadQueue?.Enqueue(() => OpenSfuDownstreamConnection(sfuDownstreamConnection.RemoteConnectionInfo));
                    }
                };

                // open connection
                await sfuDownstreamConnection.Open();
            }
            catch
            {
                Debug.LogError("Error while opening SFU downstream connection");
            }
        }

        private DataChannel PrepareDataChannel()
        {
            var dc = new DataChannel("data")
            {
                OnReceive = (e) =>
                {
                    if (e.DataString != null)
                    {
                        //Debug.Log($"Message received in datachannel {e.DataString}");
                        //MessageData chatMessage = new MessageData();
                        //chatMessage.Sender = e.RemoteConnectionInfo.UserAlias;
                        //chatMessage.TextMessage = e.DataString;
                        //StreamingChat.receivedMessagesQueue.Enqueue(chatMessage);
                    }

                }
            };

            return dc;
        }
        public void WriteMessageToChannel(string message)
        {
            if (_channel != null) // If the registration has not completed, the "_Channel" will be null. So we want to send messages only after registration.
            {
                _channel.SendMessage(message);
            }
            else
            {
                //Debug.LogError("Cant send data to channel because channel is null");
            }
        }
        internal void SendDataChannelMessage(string message)
        {
            try
            {
                //bool ifSended = false;
                DataChannel[] channels;
                lock (_dataChannelLock)
                {
                    channels = _dataChannels.ToArray();
                }

                channels = _dataChannels.ToArray();
                foreach (var channel in channels)
                {

                    if (channel.State == DataChannelState.Connected)
                    {
                        //Debug.Log("Text message sended");
                        channel.SendDataString(message);
                        //ifSended = true;
                        MessageData chatMessage = new MessageData();
                        chatMessage.Sender = Streaming._joinInfoBase.Name;
                        chatMessage.TextMessage = message;
                        //OnTextMessageSended?.Invoke(message);
                        Streaming.sendedMessagesToDataChannelsQueue.Enqueue(chatMessage);
                    }
                }
            }
            catch
            {
                //Debug.LogError("Error while sending data channel message.");
            }
        }

        void OnCompleteReadback(AsyncGPUReadbackRequest request)
        {
            if (request.hasError)
            {
                //Debug.Log("GPU readback error detected.");
                return;
            }

            //saved = true;
            //Texture2D tex = new Texture2D(1920, 1080, );
            //tex.LoadRawTextureData(request.GetData<byte>(), request.GetData<byte>().Length,);

            //byte[] rawTexture = tex.EncodeToJPG();
            //Debug.Log(Application.persistentDataPath + "/file.jpg");
            //File.WriteAllBytes(Application.persistentDataPath + "/file.jpg", rawTexture);
            CustomEventBehaviour.OnCustomRawTextureSetted?.Invoke(request.GetData<byte>());
        }

        private void SendImageFromLocalCamera(CustomLocalTexture2DMedia localTexture2DMedia)
        {
            Rect rect = new Rect(0, 0, videoSettings.Width, videoSettings.Height); //todo: disable hardcode
            RenderTexture renderTexture = new RenderTexture(videoSettings.Width, videoSettings.Height, 24);
            screenShot = new Texture2D(videoSettings.Width, videoSettings.Height, TextureFormat.RGBA32, false);
            localCamera.targetTexture = renderTexture;
            localCamera.Render();
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            //NativeArray<Color32>.Copy(screenShot.GetRawTextureData<Color32>(),
            //    localTexture2DMedia.Texture2D.GetRawTextureData<Color32>());
            //localTexture2DMedia.Texture2D.Apply();

            var tempTexture = localTexture2DMedia.Texture2D;
            var tempScreenshot = screenShot;
            try
            {
                localTexture2DMedia.Texture2D = screenShot;
            }
            catch
            {
                //Debug.LogError("Error while handling texture2D data.");
            }
            MonoBehaviour.Destroy(tempTexture);
            MonoBehaviour.Destroy(tempScreenshot);
            localCamera.targetTexture = null;
            RenderTexture.active = null;
            MonoBehaviour.Destroy(renderTexture);
        }

        private void SendImageFromRenderTexture(CustomLocalTexture2DMedia localTexture2DMedia, RenderTexture renderTexture)
        {
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            screenShot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            //NativeArray<Color32>.Copy(screenShot.GetRawTextureData<Color32>(),
            //    localTexture2DMedia.Texture2D.GetRawTextureData<Color32>());
            //localTexture2DMedia.Texture2D.Apply();

            var tempTexture = localTexture2DMedia.Texture2D;
            var tempScreenshot = screenShot;
            localTexture2DMedia.Texture2D = screenShot;
            MonoBehaviour.Destroy(tempTexture);
            MonoBehaviour.Destroy(tempScreenshot);
            RenderTexture.active = null;
        }

        #region ToggleSettings
        public void ToggleSendEncoding(int value)
        {
            if (value > _sendEncodings.Count - 1)
            {
                for (var i = 0; i < _sendEncodings.Count; i++)
                {
                    _sendEncodings[i] = true;
                }
            }
            else
            {
                for (var i = 0; i < _sendEncodings.Count; i++)
                {

                    _sendEncodings[i] = i == value;
                }
            }
            var encodings = _localMedia.VideoEncodings;
            if (encodings != null && encodings.Length > 1)
            {
                for (var i = 0; i < encodings.Length; i++)
                {
                    encodings[i].Deactivated = !_sendEncodings[i];
                }
            }
            _localMedia.VideoEncodings = encodings;
            //localPanel.SetActive(false);
        }

        public void ToggleLocalDisableVideo(string id)
        {
            ConnectionConfig config = null;
            if (_sfuUpstreamConnection != null)
            {
                config = _sfuUpstreamConnection.Config;
                config.LocalVideoDisabled = !config.LocalVideoDisabled;
                _sfuUpstreamConnection.Update(config);
            }

            if (config != null)
            {
                _localAVMaps[id + "DisableVideo"] = config.LocalVideoDisabled;
            }
            //localPanel.SetActive(false);
        }

        public void ToggleLocalDisableAudio(string id)
        {
            ConnectionConfig config = null;
            if (_sfuUpstreamConnection != null)
            {
                config = _sfuUpstreamConnection.Config;
                config.LocalAudioDisabled = !config.LocalAudioDisabled;
                _sfuUpstreamConnection.Update(config);
            }
            if (config != null)
            {
                _localAVMaps[id + "DisableAudio"] = config.LocalAudioDisabled;
            }
        }

        private void ToggleMuteVideo(string id)
        {
            ConnectionConfig config = null;
            if (_sfuUpstreamConnection != null)
            {
                config = _sfuUpstreamConnection.Config;
                config.LocalVideoMuted = !config.LocalVideoMuted;
                _sfuUpstreamConnection.Update(config);
            }

            _localAVMaps[id + "MuteVideo"] = config.LocalVideoMuted;
            //localPanel.SetActive(false);
        }

        private void ToggleMuteAudio(string id)
        {
            ConnectionConfig config = null;
            if (_sfuUpstreamConnection != null)
            {
                config = _sfuUpstreamConnection.Config;
                config.LocalAudioMuted = !config.LocalAudioMuted;
                _sfuUpstreamConnection.Update(config);
            }
            _localAVMaps[id + "MuteAudio"] = config.LocalAudioMuted;
            //localPanel.SetActive(false);
        }
        #endregion
    }
}
