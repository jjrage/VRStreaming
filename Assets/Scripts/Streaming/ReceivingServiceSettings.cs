using UnityEngine;

namespace StreamingLibrary
{
    public class ReceivingServiceSettings
    {
        /// <summary>
        /// Material used for displaying remote media.
        /// </summary>
        internal Material MaterialForDisplay { get; private set; }
        /// <summary>
        /// Material name for cases where it's custom.
        /// </summary>
        internal string MaterialName { get; private set; }
        /// <summary>
        /// Prefered audio settings.
        /// </summary>
        public AudioSettings AudioSettings { get; set; }
        public bool ReceiveOwnMessages { get; set; }

        public ReceivingServiceSettings(Material materialForDisplay, string materialName = "_MainTex")
        {
            MaterialForDisplay = materialForDisplay;
            MaterialName = materialName;
        }
        public ReceivingServiceSettings(Material materialForDisplay, AudioSettings audioSettings, string materialName = "_MainTex")
        {
            MaterialForDisplay = materialForDisplay;
            MaterialName = materialName;
            AudioSettings = audioSettings;
        }
    }
}
