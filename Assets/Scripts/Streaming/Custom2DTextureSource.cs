#region Assembly FM.LiveSwitch.Unity, Version=1.11.3.40505, Culture=neutral, PublicKeyToken=null
// C:\Unity\Projects\LiveSwitchStreaming\Assets\Plugins\FM.LiveSwitch.Unity.dll
// Decompiled with ICSharpCode.Decompiler 4.0.0.4521
#endregion

using FM.LiveSwitch;
using System;
using UnityEngine;


//
// Summary:
//     /// A Texture2D-based video source. ///
public class Custom2DTextureSource : CustomTexture2DSourceBase
{
    //
    // Summary:
    //     /// Gets a label that identifies this class. ///
    public override string Label => "Unity Texture2D Source";

    //
    // Summary:
    //     /// Gets or sets the underlying Texture2D. ///
    public Texture2D Texture2D
    {
        get;
        set;
    }

    //
    // Summary:
    //     /// Initializes a new instance of the FM.LiveSwitch.Unity.Texture2DSource class.
    //     ///
    public Custom2DTextureSource(Size size, int targetFrameRate)
        : base(VideoFormat.Rgba, GetCustomVideoConfig(size, targetFrameRate))
    {
    }

    //
    // Summary:
    //     /// Initializes a new instance of the FM.LiveSwitch.Unity.Texture2DSource class.
    //     ///
    public Custom2DTextureSource(VideoFormat outputFormat)
        : this(outputFormat, Application.targetFrameRate)
    {
        base.GameEngineFrameRate = true;
    }

    //
    // Summary:
    //     /// Initializes a new instance of the FM.LiveSwitch.Unity.Texture2DSource class.
    //     ///
    public Custom2DTextureSource(VideoFormat outputFormat, int targetFrameRate)
        : base(outputFormat, GetScreenConfig(null, targetFrameRate))
    {
    }

    //
    // Summary:
    //     /// Initializes a new instance of the FM.LiveSwitch.Unity.Texture2DSource class.
    //     ///
    public Custom2DTextureSource(Texture2D texture2D)
        : this(texture2D, Application.targetFrameRate)
    {
        base.GameEngineFrameRate = true;
    }

    //
    // Summary:
    //     /// Initializes a new instance of the FM.LiveSwitch.Unity.Texture2DSource class.
    //     ///
    public Custom2DTextureSource(Texture2D texture2D, int targetFrameRate)
        : base(GetOutputFormat(texture2D), GetScreenConfig(texture2D, targetFrameRate))
    {
        Texture2D = texture2D;
    }

    private static ScreenConfig GetScreenConfig(Texture2D texture2D, int targetFrameRate)
    {
        return new ScreenConfig(Point.Empty, new Size((texture2D == null) ? Screen.width : texture2D.width, (texture2D == null) ? Screen.height : texture2D.height), CustomTexture2DSourceBase.ProcessTargetFrameRate(targetFrameRate));
    }

    private static ScreenConfig GetCustomVideoConfig(Size size, int targetFrameRate)
    {
        return new ScreenConfig(Point.Empty, size, CustomTexture2DSourceBase.ProcessTargetFrameRate(targetFrameRate));
    }

    private static VideoFormat GetOutputFormat(Texture2D texture2D)
    {
        switch (texture2D.format)
        {
            case TextureFormat.RGBA32:
                return VideoFormat.Rgba;
            case TextureFormat.BGRA32:
                return VideoFormat.Bgra;
            case TextureFormat.ARGB32:
                return VideoFormat.Argb;
            case TextureFormat.RGB24:
                return VideoFormat.Rgb;
            default:
                throw new Exception("Texture2D is using unsupported TextureFormat: " + texture2D.format.ToString());
        }
    }

    //
    // Summary:
    //     /// Gets the current Texture2D. ///
    protected override Texture2D GetTexture2D()
    {
        return Texture2D;
    }

    //public override void CaptureAndRaiseFrameMethod()
    //{
    //    base.CaptureAndRaiseFrameMethod();
    //}
}

