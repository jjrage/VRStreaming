using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingLibrary
{
    public class AudioSettings
    {
        public AudioSettings(int bitrate)
        {
            ClockRate = 48000;
            ChannelCount = 2;
            Bitrate = bitrate;
        }

        public AudioSettings()
        {
            ClockRate = 48000;
            ChannelCount = 2;
            Bitrate = 510;
        }

        internal int ClockRate { get; }
        internal int ChannelCount { get; }
        internal int Bitrate { get; }
    }
}
