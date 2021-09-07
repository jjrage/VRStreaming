using FM.LiveSwitch;
using FM.LiveSwitch.Unity;
using StreamingLibrary;
using StreamingLibraryInternal;
using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;


public abstract class CustomTexture2DSourceBase : CustomScreenSourceBase
{
    private bool needLoger = false;
    private static ILog _log = Log.GetLogger(typeof(CustomTexture2DSourceBase));

    private int _BytesPerPixel;

    private double _MinFrameTickInterval;

    private GameObject _GameObject;

    private InternalEventBehaviour _EventBehaviour;

    private int _Width;

    private int _Height;

    private int _Stride;

    private byte[] _InputBytes;

    private byte[] _OutputBytes;

    private volatile bool _RaisingFrame;

    private long _LastRaiseFrameTimestamp = -1L;

    protected bool GameEngineFrameRate
    {
        get;
        set;
    }

    protected static int ProcessTargetFrameRate(int targetFrameRate)
    {
        if (targetFrameRate <= 0)
        {
            targetFrameRate = 30;
        }

        int targetFrameRate2 = Application.targetFrameRate;
        if (targetFrameRate2 > 0 && targetFrameRate >= targetFrameRate2)
        {
            targetFrameRate = targetFrameRate2;
        }

        return targetFrameRate;
    }

    public CustomTexture2DSourceBase(VideoFormat outputFormat, ScreenConfig targetConfig)
        : base(outputFormat, targetConfig)
    {
        if (outputFormat == null)
        {
            return;
            //throw new Exception("Output format cannot be null.");
        }

        if (outputFormat.IsRgbType)
        {
            _BytesPerPixel = 3;
        }
        else
        {
            if (!outputFormat.IsRgbaType)
            {
                return;
                //throw new Exception("Unsupported output format: " + outputFormat.Name);
            }

            _BytesPerPixel = 4;
        }

        _MinFrameTickInterval = (int)((double)Constants.TicksPerSecond / targetConfig.FrameRate);
        _GameObject = new GameObject();
        _EventBehaviour = _GameObject.AddComponent<InternalEventBehaviour>();
    }

    protected override void DoDestroy()
    {
        if (_EventBehaviour != null)
        {
            UnityEngine.Object.Destroy(_EventBehaviour);
            _EventBehaviour = null;
        }

        if (_GameObject != null)
        {
            UnityEngine.Object.Destroy(_GameObject);
            _GameObject = null;
        }

        base.DoDestroy();
    }

    protected override Future<object> DoStart()
    {
        Promise<object> promise = new Promise<object>();
        try
        {
            base.Config = base.TargetConfig;
            //_EventBehaviour.OnLateUpdate += LateUpdate;
            CustomEventBehaviour.OnCustomRawTextureSetted += CaptureAndRaiseFrameMethodRaw;
            promise.Resolve(true);
            return promise;
        }
        catch (Exception exception)
        {
            promise.Reject(exception);
            return promise;
        }
    }

    protected override Future<object> DoStop()
    {
        Promise<object> promise = new Promise<object>();
        try
        {
            //_EventBehaviour.OnLateUpdate -= LateUpdate;
            //Streaming.OnFrameSetted -= CaptureAndRaiseFrameMethod;
            CustomEventBehaviour.OnCustomRawTextureSetted -= CaptureAndRaiseFrameMethodRaw;
            promise.Resolve(true);
            return promise;
        }
        catch (Exception exception)
        {
            promise.Reject(exception);
            return promise;
        }
    }

    private void LateUpdate()
    {
        _EventBehaviour.StartCoroutine(CaptureAndRaiseFrame());
    }
    private IEnumerator CaptureAndRaiseFrame()
    {
        //Debug.Log("raise frame");
        CustomTexture2DSourceBase texture2DSourceBase = this;
        yield return new WaitForEndOfFrame();
        if (_RaisingFrame)
        {
            if (needLoger)
            {
                _log.Debug("Discarding frame. A previous frame is being raised.");
            }

            //Debug.Log("Discarding frame. A previous frame is being raised.");
            yield break;
        }

        long systemTimestamp = ManagedStopwatch.GetTimestamp();
        if (!GameEngineFrameRate && _LastRaiseFrameTimestamp != -1 && !((double)(systemTimestamp - _LastRaiseFrameTimestamp) > _MinFrameTickInterval))
        {
            yield break;
        }

        Texture2D texture2D = GetTexture2D();
        if (!(texture2D != null))
        {
            yield break;
        }

        int width = texture2D.width;
        int height = texture2D.height;
        if (width != _Width || height != _Height)
        {
            _Width = width;
            _Height = height;
            _Stride = _Width * _BytesPerPixel;
            //Debug.Log( $"Capture is using resolution {_Width}x{_Height}.");
            _InputBytes = new byte[_Stride * _Height];
            _OutputBytes = new byte[_Stride * _Height];
        }

        texture2D.GetRawTextureData<byte>().CopyTo(_InputBytes);
        int stride = _Stride;
        byte[] inputBytes = _InputBytes;
        byte[] outputBytes = _OutputBytes;
        _RaisingFrame = true;
        ManagedThread.Dispatch(delegate
        {
            try
            {
                ImageUtility.VerticalMirror(inputBytes, outputBytes, width, height, stride);
                texture2DSourceBase.RaiseFrame(new VideoFrame(new VideoBuffer(width, height, DataBuffer.Wrap(outputBytes), texture2DSourceBase.OutputFormat))
                {
                    SystemTimestamp = systemTimestamp
                });
                texture2DSourceBase._LastRaiseFrameTimestamp = systemTimestamp;
            }
            catch (Exception ex)
            {
                if (needLoger)
                {
                    _log.Debug("Could not raise frame.",ex);
                }
                //Debug.LogError("Could not raise frame.");
            }
            finally
            {
                texture2DSourceBase._RaisingFrame = false;
                UnityEngine.Object.Destroy(texture2D);
            }
        });
    }

protected abstract Texture2D GetTexture2D();

    private IEnumerator CaptureAndRaiseFrame(Texture2D capturedTexture)
    {
        CustomTexture2DSourceBase texture2DSourceBase = this;
        yield return new WaitForEndOfFrame();
        if (_RaisingFrame)
        {
            if (needLoger)
            {
                _log.Debug("Discarding frame. A previous frame is being raised.");
            }
            yield break;
        }

        long systemTimestamp = ManagedStopwatch.GetTimestamp();
        if (!GameEngineFrameRate && _LastRaiseFrameTimestamp != -1 && !((double)(systemTimestamp - _LastRaiseFrameTimestamp) > _MinFrameTickInterval))
        {
            yield break;
        }

        Texture2D texture2D = capturedTexture;
        if (!(texture2D != null))
        {
            yield break;
        }

        int width = texture2D.width;
        int height = texture2D.height;
        if (width != _Width || height != _Height)
        {
            _Width = width;
            _Height = height;
            _Stride = _Width * _BytesPerPixel;
            //Debug.Log($"Capture is using resolution {_Width}x{_Height}.");
            _InputBytes = new byte[_Stride * _Height];
            _OutputBytes = new byte[_Stride * _Height];
        }

        texture2D.GetRawTextureData<byte>().CopyTo(_InputBytes);
        int stride = _Stride;
        byte[] inputBytes = _InputBytes;
        byte[] outputBytes = _OutputBytes;
        _RaisingFrame = true;
        ManagedThread.Dispatch(delegate
        {
            try
            {
                ImageUtility.VerticalMirror(inputBytes, outputBytes, width, height, stride);
                texture2DSourceBase.RaiseFrame(new VideoFrame(new VideoBuffer(width, height, DataBuffer.Wrap(outputBytes), texture2DSourceBase.OutputFormat))
                {
                    SystemTimestamp = systemTimestamp
                });
                texture2DSourceBase._LastRaiseFrameTimestamp = systemTimestamp;
            }
            catch (Exception ex)
            {
                if (needLoger)
                {
                    _log.Debug("Could not raise frame.", ex);
                }
                //Debug.LogError("Could not raise frame.");
            }
            finally
            {
                texture2DSourceBase._RaisingFrame = false;
                UnityEngine.Object.Destroy(texture2D);
            }
        });
        //Debug.Log("[video:Library] CaptureAndRaiseFrame coroutine ended");
    }

    public void CaptureAndRaiseFrameMethod(Texture2D capturedTexture)
    {
        CustomTexture2DSourceBase texture2DSourceBase = this;

        if (_RaisingFrame)
        {
            if (needLoger)
            {
                _log.Debug("Discarding frame. A previous frame is being raised.");
            }
            return;
        }

        long systemTimestamp = ManagedStopwatch.GetTimestamp();

        if (!GameEngineFrameRate && _LastRaiseFrameTimestamp != -1 && !((double)(systemTimestamp - _LastRaiseFrameTimestamp) > _MinFrameTickInterval))
        {
            return;
        }

        Texture2D texture2D = capturedTexture;

        if (!(texture2D != null))
        {
            return;
        }

        int width = texture2D.width;
        int height = texture2D.height;
        if (width != _Width || height != _Height)
        {
            _Width = width;
            _Height = height;
            _Stride = _Width * _BytesPerPixel;
            _InputBytes = new byte[_Stride * _Height];
            _OutputBytes = new byte[_Stride * _Height];
        }

        texture2D.GetRawTextureData<byte>().CopyTo(_InputBytes);
        int stride = _Stride;
        byte[] inputBytes = _InputBytes;
        byte[] outputBytes = _OutputBytes;
        _RaisingFrame = true;

        ManagedThread.Dispatch(delegate
        {
            try
            {
                ImageUtility.VerticalMirror(inputBytes, outputBytes, width, height, stride);
                texture2DSourceBase.RaiseFrame(new VideoFrame(new VideoBuffer(width, height, DataBuffer.Wrap(outputBytes), texture2DSourceBase.OutputFormat))
                {
                    SystemTimestamp = systemTimestamp
                });
                texture2DSourceBase._LastRaiseFrameTimestamp = systemTimestamp;
            }
            catch (Exception ex)
            {
                if (needLoger)
                {
                    _log.Debug("Could not raise frame.", ex);
                }
                //Debug.LogError("Could not raise frame.");
            }
            finally
            {
                texture2DSourceBase._RaisingFrame = false;
                UnityEngine.Object.Destroy(texture2D);
            }
        });
    }

    public void CaptureAndRaiseFrameMethodRaw(NativeArray<byte> rawTextureData)
    {
        CustomTexture2DSourceBase texture2DSourceBase = this;
        if (_RaisingFrame)
        {
            if (needLoger)
            {
                _log.Debug("Discarding frame. A previous frame is being raised.");
            }

            return;
        }

        long systemTimestamp = ManagedStopwatch.GetTimestamp();

        if (!GameEngineFrameRate && _LastRaiseFrameTimestamp != -1 && !((double)(systemTimestamp - _LastRaiseFrameTimestamp) > _MinFrameTickInterval))
        {
            return;
        }

        int width = Streaming._streamingServiceSettings.VideoSettings.Width;
        int height = Streaming._streamingServiceSettings.VideoSettings.Height;
        if (width != _Width || height != _Height)
        {
            _Width = width;
            _Height = height;
            _Stride = _Width * _BytesPerPixel;
            _InputBytes = new byte[_Stride * _Height];
            _OutputBytes = new byte[_Stride * _Height];
        }

        int stride = _Stride;
        byte[] inputBytes = rawTextureData.ToArray();
        byte[] outputBytes = _OutputBytes;
        _RaisingFrame = true;
        ManagedThread.Dispatch(delegate
        {
            try
            {
                ImageUtility.VerticalMirror(inputBytes, outputBytes, width, height, stride);
                texture2DSourceBase.RaiseFrame(new VideoFrame(new VideoBuffer(width, height, DataBuffer.Wrap(outputBytes), texture2DSourceBase.OutputFormat))
                {
                    SystemTimestamp = systemTimestamp
                });

                texture2DSourceBase._LastRaiseFrameTimestamp = systemTimestamp;
            }
            catch (Exception ex)
            {
                if (needLoger)
                {
                    _log.Debug("Could not raise frame.", ex);
                }
                //Debug.LogError("Could not raise frame.");
            }
            finally
            {
                texture2DSourceBase._RaisingFrame = false;
            }
        });
    }
}