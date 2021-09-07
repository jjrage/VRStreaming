using FM.LiveSwitch;
using System.Collections.Generic;

public class AudioEncodingConfig : EncodingConfig
{
    public AudioEncodingConfig()
    {
    }

    public AudioEncodingConfig(EncodingInfo encoding)
        : base(encoding)
    {
    }

    public string ToJson()
    {
        return ToJson(this);
    }

    public static string ToJson(AudioEncodingConfig encodingConfig)
    {
        return JsonSerializer.SerializeObject(encodingConfig, delegate (AudioEncodingConfig info, Dictionary<string, string> dict)
        {
            info.SerializeProperties(dict);
        });
    }

    public static AudioEncodingConfig FromJson(string encodingConfigJson)
    {
        return JsonSerializer.DeserializeObject(encodingConfigJson, () => new AudioEncodingConfig(), delegate (AudioEncodingConfig info, string key, string value)
        {
            info.DeserializeProperties(key, value);
        });
    }

    public static string ToJsonArray(AudioEncodingConfig[] encodingConfigs)
    {
        return JsonSerializer.SerializeObjectArray(encodingConfigs, ToJson);
    }

    public static AudioEncodingConfig[] FromJsonArray(string encodingConfigsJson)
    {
        return JsonSerializer.DeserializeObjectArray(encodingConfigsJson, FromJson)?.ToArray();
    }

    public override string ToString()
    {
        bool flag = false;
        List<string> list = new List<string>();
        string rtpStreamId = base.RtpStreamId;
        if (rtpStreamId != null)
        {
            list.Add($"RTP Stream ID: {rtpStreamId}");
        }

        long synchronizationSource = base.SynchronizationSource;
        if (synchronizationSource != -1)
        {
            list.Add($"Synchronization Source: {synchronizationSource.ToString()}");
        }

        bool deactivated = base.Deactivated;
        if (deactivated)
        {
            list.Add($"Deactivated: {deactivated.ToString()}");
        }

        int bitrate = base.Bitrate;
        if (bitrate != -1)
        {
            flag = true;
            list.Add($"Bitrate: {bitrate.ToString()}");
        }

        string text = string.Join(", ", list.ToArray());
        if (!flag)
        {
            text = $"{text} [Unrestricted]".Trim();
        }

        return text;
    }
}