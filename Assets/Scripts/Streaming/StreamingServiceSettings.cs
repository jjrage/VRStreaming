using UnityEngine;
using UnityEngine.UI;

namespace StreamingLibrary
{
    public class StreamingServiceSettings
    {
        /// <summary>
        /// Camera from local scene that capturing video data into render texture.
        /// </summary>
        public Camera LocalCamera { get; set; }
        /// <summary>
        /// Render texture with captured data. In case if render texture won't be setted streaming service will use render texture assigned to local camera.
        /// </summary>
        public RenderTexture RenderTexture { get; set; }
        /// <summary>
        /// Prefered video settings.
        /// </summary>
        public VideoSettings VideoSettings { get; set; }
        /// <summary>
        /// Prefered audio settings.
        /// </summary>
        public AudioSettings AudioSettings { get; set; }
        public Dropdown AudioSelection { get; set; }
        public bool ReceiveOwnMessages { get; set; }
    }
}
