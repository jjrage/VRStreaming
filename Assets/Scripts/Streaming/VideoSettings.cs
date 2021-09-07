using FM.LiveSwitch;
using System;
using System.Collections.Generic;
using System.Text;

public class VideoSettings
    {
        public VideoSettings(int height, int width, int fps, int bitrate)
        {
            Height = height;
            Width = width;
            FPS = fps;
            Bitrate = bitrate;
        }

        public VideoSettings()
        {
            Height = 1080;
            Width = 1920;
            FPS = 60;
            Bitrate = 3000;
        }

        /// <summary>
        /// Height of captured data. 
        /// </summary>
        internal int Height { get; }
        /// <summary>
        /// Width of captured data. 
        /// </summary>
        internal int Width { get; }
        /// <summary>
        /// Prefered FPS value. This approximate value and it depends on general perfomance. 
        /// </summary>
        internal int FPS { get; }
        /// <summary>
        /// Prefered Bitrate value. This approximate value and it depends on general perfomance. 
        /// </summary>
        internal int Bitrate { get; }
    }
