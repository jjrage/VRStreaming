using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using UnityEngine;

namespace StreamingLibrary
{
    public static class CustomEventBehaviour
    {
        public static Action<Texture2D> OnCustomTextureSetted;
        public static Action<NativeArray<byte>> OnCustomRawTextureSetted;
    }
}
