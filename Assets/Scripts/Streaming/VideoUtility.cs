using FM.LiveSwitch;

internal class VideoUtility
{
    public static double BitratePowerScale => 0.75;

    public static int GetBitrate(int preferredBitrate, int width, int height, double frameRate, double bitsPerPixel)
    {
        if (width > 0 && height > 0 && frameRate > 0.0 && bitsPerPixel > 0.0)
        {
            return GetBitrate(width * height, frameRate, bitsPerPixel);
        }

        return preferredBitrate;
    }

    public static int GetBitrate(int pixelCount, double frameRate, double bitsPerPixel)
    {
        int num = 480;
        int num2 = 640 * num;
        double num3 = (double)num2 * frameRate * bitsPerPixel / 1000.0;
        return (int)MathAssistant.Ceil(MathAssistant.Pow((double)pixelCount / (double)num2, BitratePowerScale) * num3);
    }

    public static int GetPixelCount(int bitrate, double frameRate, double bitsPerPixel)
    {
        int num = 480;
        int num2 = 640 * num;
        double num3 = (double)num2 * frameRate * bitsPerPixel / 1000.0;
        return (int)MathAssistant.Round(MathAssistant.Pow((double)bitrate / num3, 1.0 / BitratePowerScale) * (double)num2);
    }

    public static VideoDegradationPreference ProcessDegradationPreference(VideoDegradationPreference degradationPreference, VideoType type)
    {
        if (degradationPreference != VideoDegradationPreference.Automatic)
        {
            return degradationPreference;
        }

        switch (type)
        {
            case VideoType.Camera:
                return VideoDegradationPreference.Resolution;
            case VideoType.Screen:
                return VideoDegradationPreference.FrameRate;
            default:
                return VideoDegradationPreference.Balanced;
        }
    }

    public static VideoEncodingConfig GetEncodingConfig(VideoDegradationPreference degradationPreference, double multiplier, double frameRate)
    {
        VideoEncodingConfig videoEncodingConfig = new VideoEncodingConfig();
        UpdateEncodingConfig(videoEncodingConfig, degradationPreference, multiplier, frameRate);
        return videoEncodingConfig;
    }

    public static void UpdateEncodingConfig(VideoEncodingConfig encodingConfig, VideoDegradationPreference degradationPreference, double multiplier, double frameRate)
    {
        if (degradationPreference == VideoDegradationPreference.Balanced)
        {
            multiplier = MathAssistant.Sqrt(multiplier);
        }

        if (frameRate > 0.0)
        {
            if (degradationPreference == VideoDegradationPreference.FrameRate || degradationPreference == VideoDegradationPreference.Balanced)
            {
                encodingConfig.FrameRate = frameRate * multiplier;
            }
            else
            {
                encodingConfig.FrameRate = frameRate;
            }
        }

        if (degradationPreference == VideoDegradationPreference.Resolution || degradationPreference == VideoDegradationPreference.Balanced)
        {
            encodingConfig.Scale = MathAssistant.Sqrt(multiplier);
        }
        else
        {
            encodingConfig.Scale = 1.0;
        }
    }
}