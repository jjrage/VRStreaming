using FM.LiveSwitch;
using FM.LiveSwitch.Unity;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace StreamingLibrary
{
    public class CustomRectTransformSink : ViewSink<RectTransform>
    {
        private static ILog _log = Log.GetLogger(typeof(RectTransformSink));

        private GameObject _OuterGameObject;

        private GameObject _InnerGameObject;

        private RectTransform _OuterTransform;

        private RectTransform _InnerTransform;

        private EventBehaviour _CoreBehaviour;

        private RawImage _RawImage;

        private Texture2D _Texture;
        private MonoBehaviour _monoBehaviour;

        private ConcurrentQueue<VideoBuffer> _VideoBuffers;

        private bool _LastViewMirror;

        private LayoutScale _LastViewScale = LayoutScale.Stretch;

        private double _LastOuterAspectRatio;

        private double _LastInnerAspectRatio;

        public override string Label => "Unity RectTransform Sink";

        public override RectTransform View => _OuterTransform;

        public Texture2D Texture => _Texture;

        public GameObject GameObject => _OuterGameObject;

        public override LayoutScale ViewScale
        {
            get;
            set;
        }

        public override bool ViewMirror
        {
            get;
            set;
        }

        public CustomRectTransformSink()
            : this(VideoFormat.Rgb)
        {
        }

        public CustomRectTransformSink(VideoFormat inputFormat)
            : base(inputFormat)
        {
            if (!GetTextureFormat(inputFormat.Name).HasValue)
            {
                throw new Exception("Unsupported format. Supported formats: ARGB, RGBA, BGRA, RGB");
            }

            ViewScale = LayoutScale.Contain;
            ViewMirror = false;
            _OuterGameObject = new GameObject();
            _OuterGameObject.AddComponent<CanvasRenderer>();
            _OuterTransform = _OuterGameObject.AddComponent<RectTransform>();
            _InnerGameObject = new GameObject();
            _RawImage = _InnerGameObject.AddComponent<RawImage>();
            _InnerTransform = _RawImage.rectTransform;
            _InnerTransform.SetParent(_OuterTransform, worldPositionStays: false);
            _InnerTransform.localPosition = Vector3.zero;
            _Texture = new Texture2D(0, 0, TextureFormat.RGBA32, mipChain: false);
            _RawImage.texture = _Texture;
            _RawImage.transform.localRotation = Quaternion.Euler(0f, 180f, 180f);
            _VideoBuffers = new ConcurrentQueue<VideoBuffer>();
            _CoreBehaviour = _OuterGameObject.AddComponent<EventBehaviour>();
            _monoBehaviour = _InnerGameObject.GetComponent<MonoBehaviour>();
            _CoreBehaviour.OnFixedUpdate += Update;
            //_CoreBehaviour.OnLateUpdate += UpdateGPU;
            //_CoreBehaviour.OnLateUpdate += ()=> { _monoBehaviour.StartCoroutine(UpdateGPURoutine()); };
        }

        protected override void DoDestroy()
        {
            base.DoDestroy();
            if (_CoreBehaviour != null)
            {
                _CoreBehaviour.OnFixedUpdate -= Update;
                //_CoreBehaviour.OnLateUpdate -= UpdateGPU;
                //_CoreBehaviour.OnLateUpdate -= () => { _monoBehaviour.StartCoroutine(UpdateGPURoutine()); };
                UnityEngine.Object.Destroy(_CoreBehaviour);
                _CoreBehaviour = null;
            }

            if (_InnerGameObject != null)
            {
                UnityEngine.Object.Destroy(_InnerGameObject);
                _InnerGameObject = null;
            }

            if (_OuterGameObject != null)
            {
                UnityEngine.Object.Destroy(_OuterGameObject);
                _OuterGameObject = null;
            }
        }

        private static TextureFormat? GetTextureFormat(string formatName)
        {
            if (formatName == VideoFormat.ArgbName)
            {
                return TextureFormat.ARGB32;
            }

            if (formatName == VideoFormat.RgbaName)
            {
                return TextureFormat.RGBA32;
            }

            if (formatName == VideoFormat.BgraName)
            {
                return TextureFormat.BGRA32;
            }

            if (formatName == VideoFormat.RgbName)
            {
                return TextureFormat.RGB24;
            }

            return null;
        }

        protected override void RenderBuffer(VideoBuffer inputBuffer)
        {
            _VideoBuffers.Enqueue(inputBuffer.Keep());
            while (_VideoBuffers.Count > 1)
            {
                if (_VideoBuffers.TryDequeue(out VideoBuffer result))
                {
                    result.Free();
                }
            }
        }

        //private void ApplyViewMirror()
        //{
        //    if (ViewMirror != _LastViewMirror)
        //    {
        //        Transform transform = _RawImage.transform;
        //        Vector3 localScale = transform.localScale;
        //        transform.localScale = new Vector3(0f - localScale.x, localScale.y, localScale.z);
        //        _LastViewMirror = ViewMirror;
        //    }
        //}

        //private void ApplyViewScale()
        //{
        //    float num = 1f;
        //    float num2 = 1f;
        //    if (ViewScale != LayoutScale.Stretch)
        //    {
        //        num = _OuterTransform.GetAspectRatio();
        //        num2 = _Texture.GetAspectRatio();
        //    }

        //    if (ViewScale != _LastViewScale || (double)num != _LastOuterAspectRatio || (double)num2 != _LastInnerAspectRatio)
        //    {
        //        _InnerTransform.ApplyLayoutScale(ViewScale, num, num2);
        //        _LastViewScale = ViewScale;
        //        _LastOuterAspectRatio = num;
        //        _LastInnerAspectRatio = num2;
        //    }
        //}

        private void Update()
        {
            if (!_VideoBuffers.TryDequeue(out VideoBuffer result))
            {
                return;
            }

            try
            {
                int width = result.Width;
                int height = result.Height;
                if (_Texture.width != width || _Texture.height != height)
                {
                    _Texture.Resize(width, height);
                }

                //ApplyViewMirror();
                //ApplyViewScale();
                int num = 0;
                NativeArray<byte> rawTextureData = _Texture.GetRawTextureData<byte>();
                DataBuffer[] dataBuffers = result.DataBuffers;
                //Graphics.CopyTexture()
                foreach (DataBuffer dataBuffer in dataBuffers)
                {
                    NativeArray<byte>.Copy(dataBuffer.Data, dataBuffer.Index, rawTextureData, num, dataBuffer.Length);
                    num += dataBuffer.Length;
                }

                _Texture.Apply();
            }
            finally
            {
                result.Free();
            }
        }

        private IEnumerator UpdateGPURoutine()
        {
            UpdateGPU();
            yield return new WaitForEndOfFrame();
        }
        private void UpdateGPU()
        {
            //ComputeBuffer computeBuffer = new ComputeBuffer
            AsyncGPUReadback.Request(_Texture, 0, _Texture.graphicsFormat, OnCompleteReadback);
 
        }

        void OnCompleteReadback(AsyncGPUReadbackRequest request)
        {
            if (request.hasError)
            {
                Debug.Log("GPU readback error detected.");
                Debug.Log($"request is done {request.done}");
                //Debug.Log($"request is done {request.}");
                return;
            }
            if (!_VideoBuffers.TryDequeue(out VideoBuffer result))
            {
                Debug.Log("Can't dequeue");
                return;
            }
            Debug.Log($"Dequeue success {result.ToJson()}");
            try
            {
                int width = result.Width;
                int height = result.Height;
                if (_Texture.width != width || _Texture.height != height)
                {
                    _Texture.Resize(width, height);
                }

                //ApplyViewMirror();
                //ApplyViewScale();
                int num = 0;

                NativeArray<byte> rawTextureData = request.GetData<byte>();
                Debug.Log($"Raw texture data {rawTextureData.Length}");
                DataBuffer[] dataBuffers = result.DataBuffers;
                //Graphics.CopyTexture()
                foreach (DataBuffer dataBuffer in dataBuffers)
                {
                    NativeArray<byte>.Copy(dataBuffer.Data, dataBuffer.Index, rawTextureData, num, dataBuffer.Length);
                    num += dataBuffer.Length;
                    Debug.Log($"Setted data from databuffer {num}");
                }

                //_Texture.Apply();
            }
            finally
            {
                result.Free();
            }
            //saved = true;
            //Texture2D tex = new Texture2D(1920, 1080, );
            //tex.LoadRawTextureData(request.GetData<byte>(), request.GetData<byte>().Length,);

            //byte[] rawTexture = tex.EncodeToJPG();
            //Debug.Log(Application.persistentDataPath + "/file.jpg");
            //File.WriteAllBytes(Application.persistentDataPath + "/file.jpg", rawTexture);
            //CustomEventBehaviour.OnCustomRawTextureSetted?.Invoke(request.GetData<byte>());
        }
    }
}
