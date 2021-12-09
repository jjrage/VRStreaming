using FM.LiveSwitch;
using FM.LiveSwitch.Unity;
using StreamingLibrary;
using UnityEngine;

public abstract class LocalMedia : RtcLocalMedia<UnityEngine.RectTransform>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalMedia"/> class.
    /// </summary>
    /// <param name="disableAudio">Whether to disable audio.</param>
    /// <param name="disableVideo">Whether to disable video.</param>
    /// <param name="aecContext">The AEC context, if using software echo cancellation.</param>
    public LocalMedia(bool disableAudio, bool disableVideo)
        : base(disableAudio, disableVideo)
    { }

    /// <summary>
    /// Creates an audio recorder.
    /// </summary>
    /// <param name="inputFormat">The input format.</param>
    protected override AudioSink CreateAudioRecorder(AudioFormat inputFormat)
    {
        return new FM.LiveSwitch.Matroska.AudioSink(Id + "-local-audio-" + inputFormat.Name.ToLower() + ".mkv");
    }

    /// <summary>
    /// Creates an audio source.
    /// </summary>
    /// <param name="config">The configuration.</param>
    protected override FM.LiveSwitch.AudioSource CreateAudioSource(AudioConfig config)
    {
        return new AudioClipSource(config);
    }

    /// <summary>
    /// Creates an image converter.
    /// </summary>
    /// <param name="outputFormat">The output format.</param>
    protected override VideoPipe CreateImageConverter(VideoFormat outputFormat)
    {
        return new FM.LiveSwitch.Yuv.ImageConverter(outputFormat);
    }

    /// <summary>
    /// Creates an Opus encoder.
    /// </summary>
    /// <param name="config">The configuration.</param>
    //protected override AudioEncoder CreateOpusEncoder(AudioConfig config)
    //{
    //    Debug.Log($"created opus encoder base {config.ChannelCount}, {config.ClockRate}");
    //    return new FM.LiveSwitch.Opus.Encoder(config);
    //}

    /// <summary>
    /// Creates a video recorder.
    /// </summary>
    /// <param name="inputFormat">The input format.</param>
    protected override VideoSink CreateVideoRecorder(VideoFormat inputFormat)
    {
        return new FM.LiveSwitch.Matroska.VideoSink(Id + "-local-video-" + inputFormat.Name.ToLower() + ".mkv");
    }

    /// <summary>
    /// Creates a VP8 encoder.
    /// </summary>
    protected override VideoEncoder CreateVp8Encoder()
    {
        return new FM.LiveSwitch.Vp8.Encoder();
    }

    /// <summary>
    /// Creates an H.264 encoder.
    /// </summary>
    protected override VideoEncoder CreateH264Encoder()
    {
        // OpenH264 requires a runtime download from Cisco
        // for licensing reasons, which is not currently
        // supported on Unity.
        return null;
    }

    /// <summary>
    /// Creates a VP9 encoder.
    /// </summary>
    protected override VideoEncoder CreateVp9Encoder()
    {
        return new FM.LiveSwitch.Vp9.Encoder();
    }
}

public class CustomLocalTexture2DMedia : LocalMedia
{
    private Size _size;
    private int _targetFrameRate;
    private StreamingLibrary.AudioSettings _audioSettings;
    private FM.LiveSwitch.Opus.Encoder _encoder;
    private AudioPipe _opusPacketizer;
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLocalTexture2DMedia"/> class.
    /// </summary>
    /// <param name="disableAudio">Whether to disable audio.</param>
    /// <param name="disableVideo">Whether to disable video.</param>
    /// <param name="aecContext">The AEC context, if using software echo cancellation.</param>
    public CustomLocalTexture2DMedia(bool disableAudio, bool disableVideo)
        : base(disableAudio, disableVideo)
    { }
    public void SetVideoSettings(Size size, int targetFrameRate)
    {
        _size = size;
        _targetFrameRate = targetFrameRate;
    }
    public void SetAudioSettings(StreamingLibrary.AudioSettings audioSettings)
    {
        _audioSettings = audioSettings;
    }

    /// <summary>
    /// Creates a video source.
    /// </summary>
    protected override VideoSource CreateVideoSource()
    {
        FM.LiveSwitch.VideoSource videoSource = new Custom2DTextureSource(_size, _targetFrameRate);
        return videoSource;
    }

    /// <summary>
    /// Creates a view sink.
    /// </summary>
    protected override ViewSink<UnityEngine.RectTransform> CreateViewSink()
    {
        // Texture capture doesn't generally need a preview.
        // If you want one, return a new RectTransformSink here.
        return null;
    }

    protected override AudioPipe CreateOpusPacketizer(AudioConfig config)
    {
        if (_audioSettings != null)
        {
            var audioConfig = new AudioConfig(_audioSettings.ClockRate, _audioSettings.ChannelCount);
            _opusPacketizer = base.CreateOpusPacketizer(audioConfig);
        }
        else
        {
            _opusPacketizer = base.CreateOpusPacketizer(config);
        }

        return _opusPacketizer;
    }

    protected override AudioEncoder CreateOpusEncoder(AudioConfig config)
    {
        if (_audioSettings != null)
        {
            var audioConfig = new AudioConfig(_audioSettings.ClockRate, _audioSettings.ChannelCount);
            _encoder = new FM.LiveSwitch.Opus.Encoder(audioConfig) { TargetBitrate = _audioSettings.Bitrate};
        }
        else
        {
            _encoder = new FM.LiveSwitch.Opus.Encoder(config);
        }

        return _encoder;
    }

    /// <summary>
    /// Gets or sets the underlying Texture2D.
    /// </summary>
    public UnityEngine.Texture2D Texture2D
    {
       
        get { return ((Custom2DTextureSource)VideoSource).Texture2D; }
        set { ((Custom2DTextureSource)VideoSource).Texture2D = value; }
    }
}
