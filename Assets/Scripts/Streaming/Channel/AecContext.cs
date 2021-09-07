using FM.LiveSwitch;
using FM.LiveSwitch.AudioProcessing;
using FM.LiveSwitch.Unity;
using UnityEngine;

public class AecContext : FM.LiveSwitch.AecContext
{
    //private AudioConfig _audioConfig;
    //private StreamingLibrary.AudioSettings _audioSettings;

    //public AecContext(StreamingLibrary.AudioSettings audioSettings)
    //{
    //    Debug.Log("[aec] 1");
    //    Debug.Log($"audio settings setted");
    //    _audioSettings = audioSettings;
    //    Debug.Log(_audioSettings.ChannelCount);
    //    Debug.Log("[aec] 2");
    //    SetAudioSettings(_audioSettings);
    //    Debug.Log("[aec] 3");
    //}

    //public AecContext()
    //{

    //}
    /// <summary>
    /// Creates an acoustic echo cancellation processor.
    /// </summary>
    protected override AecPipe CreateProcessor()
    {
        //Debug.Log("[aec] 5");
        //Debug.Log($"creating proccessor");
        //Debug.Log(_audioSettings.ChannelCount);
        //Debug.Log($"{_audioConfig.ChannelCount}");
        //if (_audioConfig != null)
        //{
        //    Debug.Log($"aec pocessor created with custom config {_audioConfig}");
        //    return new AecProcessor(_audioConfig, AudioClipSource.GetBufferDelay(_audioConfig) + AudioClipSink.GetBufferDelay(_audioConfig));
        //}
        //else
        //{
        //    Debug.Log($"aec pocessor created with default config");
        //    var config = new AudioConfig(16000, 1);
        //    return new AecProcessor(config, AudioClipSource.GetBufferDelay(config) + AudioClipSink.GetBufferDelay(config));
        //}

        var config = new AudioConfig(48000, 2);
        return new AecProcessor(config, AudioClipSource.GetBufferDelay(config) + AudioClipSink.GetBufferDelay(config));
    }

    /// <summary>
    /// Creates an output mixer sink.
    /// </summary>
    /// <param name="config">The configuration.</param>
    protected override AudioSink CreateOutputMixerSink(AudioConfig config)
    {
        //if (_audioConfig != null)
        //{
        //    Debug.Log($"CreateOutputMixerSink created with custom config {_audioConfig}");
        //    return new AudioClipSink(_audioConfig);
        //}
        //else
        //{
        //    Debug.Log($"CreateOutputMixerSink created with default config");
        //    return new AudioClipSink(config);
        //}
        return new AudioClipSink(config);
    }
    //public void SetAudioSettings(StreamingLibrary.AudioSettings audioSettings)
    //{
    //    Debug.Log($"audio config was created");
    //    _audioConfig = new AudioConfig(audioSettings.ClockRate, audioSettings.ChannelCount);
    //    Debug.Log("[aec] 4");
    //}
}
