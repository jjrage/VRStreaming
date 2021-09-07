using FM.LiveSwitch;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StreamingLibrary
{
    public abstract class CustomRtcRemoteMedia<TView> : FM.LiveSwitch.RemoteMedia, IViewSinkableMedia<TView, ViewSink<TView>>, IViewableMedia<TView>
    {
        private static ILog _log = Log.GetLogger(typeof(CustomRtcRemoteMedia<>));

        private AudioConfig _OpusConfig = new AudioConfig(48000, 2);

        private AudioConfig _G722Config = new AudioConfig(16000, 1);

        private AudioConfig _PcmuConfig = new AudioConfig(8000, 1);

        private AudioConfig _PcmaConfig = new AudioConfig(8000, 1);

        private List<AudioSink> _AudioRecorders = new List<AudioSink>();

        private List<VideoSink> _VideoRecorders = new List<VideoSink>();

        private object _InitializeLock = new object();

        private bool _Initialized;

        private object AudioRecordingLock = new object();

        private object VideoRecordingLock = new object();

        public bool AudioDisabled
        {
            get;
            private set;
        }

        public bool VideoDisabled
        {
            get;
            private set;
        }

        public AecContext AecContext
        {
            get;
            private set;
        }

        public bool AecDisabled
        {
            get
            {
                if (AecContext != null)
                {
                    return AudioDisabled;
                }

                return true;
            }
        }

        public ViewSink<TView> ViewSink
        {
            get;
            private set;
        }

        public TView View
        {
            get
            {
                if (ViewSink != null)
                {
                    return ViewSink.View;
                }

                return default(TView);
            }
        }

        public IAudioInput[] AudioInputs
        {
            get
            {
                if (base.AudioTrack != null)
                {
                    return base.AudioTrack.Inputs;
                }

                return null;
            }
        }

        public IVideoInput[] VideoInputs
        {
            get
            {
                if (base.VideoTrack != null)
                {
                    return base.VideoTrack.Inputs;
                }

                return null;
            }
        }

        public bool OpusDisabled
        {
            get;
            private set;
        }

        public bool G722Disabled
        {
            get;
            private set;
        }

        public bool PcmuDisabled
        {
            get;
            private set;
        }

        public bool PcmaDisabled
        {
            get;
            private set;
        }

        public bool Vp8Disabled
        {
            get;
            private set;
        }

        public bool Vp9Disabled
        {
            get;
            private set;
        }

        public bool H264Disabled
        {
            get;
            private set;
        }

        public bool H265Disabled
        {
            get;
            private set;
        }

        public AudioPipe OpusDepacketizer
        {
            get;
            private set;
        }

        public AudioDecoder OpusDecoder
        {
            get;
            private set;
        }

        public AudioPipe OpusConverter
        {
            get;
            private set;
        }

        public AudioSynchronizer OpusSynchronizer
        {
            get;
            private set;
        }

        public AudioSink OpusSink
        {
            get;
            private set;
        }

        public AudioPipe G722Depacketizer
        {
            get;
            private set;
        }

        public AudioDecoder G722Decoder
        {
            get;
            private set;
        }

        public AudioPipe G722Converter
        {
            get;
            private set;
        }

        public AudioSynchronizer G722Synchronizer
        {
            get;
            private set;
        }

        public AudioSink G722Sink
        {
            get;
            private set;
        }

        public AudioPipe PcmuDepacketizer
        {
            get;
            private set;
        }

        public AudioDecoder PcmuDecoder
        {
            get;
            private set;
        }

        public AudioPipe PcmuConverter
        {
            get;
            private set;
        }

        public AudioSynchronizer PcmuSynchronizer
        {
            get;
            private set;
        }

        public AudioSink PcmuSink
        {
            get;
            private set;
        }

        public AudioPipe PcmaDepacketizer
        {
            get;
            private set;
        }

        public AudioDecoder PcmaDecoder
        {
            get;
            private set;
        }

        public AudioPipe PcmaConverter
        {
            get;
            private set;
        }

        public AudioSynchronizer PcmaSynchronizer
        {
            get;
            private set;
        }

        public AudioSink PcmaSink
        {
            get;
            private set;
        }

        public VideoPipe Vp8Depacketizer
        {
            get;
            private set;
        }

        public VideoDecoder Vp8Decoder
        {
            get;
            private set;
        }

        public VideoPipe Vp8Converter
        {
            get;
            private set;
        }

        public VideoSynchronizer Vp8Synchronizer
        {
            get;
            private set;
        }

        public VideoPipe H264Depacketizer => Utility.FirstOrDefault(H264Depacketizers);

        public VideoPipe[] H264Depacketizers => Utility.FirstOrDefault(H264DepacketizersArray);

        public VideoPipe[][] H264DepacketizersArray
        {
            get;
            private set;
        }

        public VideoDecoder H264Decoder => Utility.FirstOrDefault(H264Decoders);

        public VideoDecoder[] H264Decoders
        {
            get;
            private set;
        }

        public VideoPipe H264Converter => Utility.FirstOrDefault(H264Converters);

        public VideoPipe[] H264Converters
        {
            get;
            private set;
        }

        public VideoSynchronizer H264Synchronizer => Utility.FirstOrDefault(H264Synchronizers);

        public VideoSynchronizer[] H264Synchronizers
        {
            get;
            private set;
        }

        public VideoPipe Vp9Depacketizer
        {
            get;
            private set;
        }

        public VideoDecoder Vp9Decoder
        {
            get;
            private set;
        }

        public VideoPipe Vp9Converter
        {
            get;
            private set;
        }

        public VideoSynchronizer Vp9Synchronizer
        {
            get;
            private set;
        }

        public VideoPipe H265Depacketizer
        {
            get;
            private set;
        }

        public VideoDecoder H265Decoder
        {
            get;
            private set;
        }

        public VideoPipe H265Converter
        {
            get;
            private set;
        }

        public VideoSynchronizer H265Synchronizer
        {
            get;
            private set;
        }

        public AudioPipe ActiveAudioDepacketizer
        {
            get;
            private set;
        }

        public AudioDecoder ActiveAudioDecoder
        {
            get;
            private set;
        }

        public AudioSynchronizer ActiveAudioSynchronizer
        {
            get;
            private set;
        }

        public AudioPipe ActiveAudioConverter
        {
            get;
            private set;
        }

        public AudioSink ActiveAudioSink
        {
            get;
            private set;
        }

        public VideoPipe ActiveVideoDepacketizer
        {
            get;
            private set;
        }

        public VideoDecoder ActiveVideoDecoder
        {
            get;
            private set;
        }

        public VideoSynchronizer ActiveVideoSynchronizer
        {
            get;
            private set;
        }

        public VideoPipe ActiveVideoConverter
        {
            get;
            private set;
        }

        public event Action1<AudioPipe> OnActiveAudioDepacketizerChange;

        public event Action1<AudioDecoder> OnActiveAudioDecoderChange;

        public event Action1<AudioSynchronizer> OnActiveAudioSynchronizerChange;

        public event Action1<AudioPipe> OnActiveAudioConverterChange;

        public event Action1<AudioSink> OnActiveAudioSinkChange;

        public event Action1<VideoPipe> OnActiveVideoDepacketizerChange;

        public event Action1<VideoDecoder> OnActiveVideoDecoderChange;

        public event Action1<VideoSynchronizer> OnActiveVideoSynchronizerChange;

        public event Action1<VideoPipe> OnActiveVideoConverterChange;

        public CustomRtcRemoteMedia(bool disableAudio, bool disableVideo, AecContext aecContext)
        {
            if (aecContext != null && aecContext.Processor != null && aecContext.Processor.State != MediaPipeState.Initialized)
            {
                //_log.Warn("Remote media received a reference to a destroyed AEC context. AEC will be disabled.");
                aecContext = null;
            }

            AudioDisabled = disableAudio;
            VideoDisabled = disableVideo;
            AecContext = aecContext;
        }

        public CustomRtcRemoteMedia(bool disableAudio, bool disableVideo)
            : this(disableAudio, disableVideo, (AecContext)null)
        {
        }

        private AudioTrack CreateAudioTrack(AudioPipe depacketizer, AudioDecoder decoder, AudioPipe converter, AudioSynchronizer synchronizer, AudioSink sink)
        {
            if (depacketizer == null)
            {
                throw new Exception("Can't create remote audio track. No depacketizer.");
            }

            AudioTrack audioTrack = new AudioTrack(depacketizer);
            depacketizer.OnProcessFrame += delegate
            {
                if (ActiveAudioDepacketizer != depacketizer)
                {
                    ActiveAudioDepacketizer = depacketizer;
                    this.OnActiveAudioDepacketizerChange?.Invoke(depacketizer);
                }
            };
            List<AudioTrack> list = new List<AudioTrack>();
            AudioSink audioRecorder = GetAudioRecorder(depacketizer.OutputFormat);
            if (audioRecorder != null)
            {
                list.Add(new AudioTrack(audioRecorder));
            }

            if (decoder != null)
            {
                AudioTrack audioTrack2 = new AudioTrack(decoder);
                decoder.OnProcessFrame += delegate
                {
                    if (ActiveAudioDecoder != decoder)
                    {
                        ActiveAudioDecoder = decoder;
                        this.OnActiveAudioDecoderChange?.Invoke(decoder);
                    }
                };
                if (!AecDisabled)
                {
                    audioTrack2 = audioTrack2.Next(converter);
                    converter.OnProcessFrame += delegate
                    {
                        if (ActiveAudioConverter != converter)
                        {
                            ActiveAudioConverter = converter;
                            this.OnActiveAudioConverterChange?.Invoke(converter);
                        }
                    };
                }

                if (synchronizer != null)
                {
                    audioTrack2 = audioTrack2.Next(synchronizer);
                    synchronizer.OnProcessFrame += delegate
                    {
                        if (ActiveAudioSynchronizer != synchronizer)
                        {
                            ActiveAudioSynchronizer = synchronizer;
                            this.OnActiveAudioSynchronizerChange?.Invoke(synchronizer);
                        }
                    };
                }

                if (AecDisabled)
                {
                    if (sink != null)
                    {
                        audioTrack2 = audioTrack2.Next(sink);
                        sink.OnProcessFrame += delegate
                        {
                            if (ActiveAudioSink != sink)
                            {
                                ActiveAudioSink = sink;
                                this.OnActiveAudioSinkChange?.Invoke(sink);
                            }
                        };
                    }
                }
                else
                {
                    //audioTrack2 = audioTrack2.Next(new AudioTrack[1]
                    //{
                    //    AecContext.OutputTrack
                    //});
                    ActiveAudioSink = AecContext.OutputMixerSink;
                }

                list.Add(audioTrack2);
            }

            return audioTrack.Next(list.ToArray());
        }

        private VideoTrack CreateVideoTrack(VideoPipe[] depacketizers, VideoDecoder decoder, VideoPipe converter, VideoSynchronizer synchronizer)
        {
            if (depacketizers == null || depacketizers.Length == 0)
            {
                throw new Exception("Can't create remote video track. No depacketizer.");
            }

            List<VideoTrack> list = new List<VideoTrack>();
            foreach (VideoPipe depacketizer in depacketizers)
            {
                list.Add(CreateVideoBranch(depacketizer));
            }

            VideoTrack videoTrack = new VideoTrack(list.ToArray());
            if (decoder != null)
            {
                VideoTrack videoTrack2 = new VideoTrack(decoder);
                decoder.OnProcessFrame += delegate
                {
                    if (ActiveVideoDecoder != decoder)
                    {
                        ActiveVideoDecoder = decoder;
                        this.OnActiveVideoDecoderChange?.Invoke(decoder);
                    }
                };
                if (converter != null)
                {
                    videoTrack2 = videoTrack2.Next(converter);
                    converter.OnProcessFrame += delegate
                    {
                        if (ActiveVideoConverter != converter)
                        {
                            ActiveVideoConverter = converter;
                            this.OnActiveVideoConverterChange?.Invoke(converter);
                        }
                    };
                }

                if (synchronizer != null)
                {
                    videoTrack2 = videoTrack2.Next(synchronizer);
                    synchronizer.OnProcessFrame += delegate
                    {
                        if (ActiveVideoSynchronizer != synchronizer)
                        {
                            ActiveVideoSynchronizer = synchronizer;
                            this.OnActiveVideoSynchronizerChange?.Invoke(synchronizer);
                        }
                    };
                }

                videoTrack = videoTrack.Next(new IdentityVideoPipe(decoder.InputFormat)).Next(new VideoTrack[1]
                {
                    videoTrack2
                });
            }

            return videoTrack;
        }

        private VideoTrack CreateVideoBranch(VideoPipe depacketizer)
        {
            depacketizer.OnProcessFrame += delegate
            {
                if (ActiveVideoDepacketizer != depacketizer)
                {
                    ActiveVideoDepacketizer = depacketizer;
                    this.OnActiveVideoDepacketizerChange?.Invoke(depacketizer);
                }
            };
            List<VideoTrack> list = new List<VideoTrack>();
            VideoSink videoRecorder = GetVideoRecorder(depacketizer.OutputFormat);
            if (videoRecorder != null)
            {
                list.Add(new VideoTrack(videoRecorder));
            }

            list.Add(new VideoTrack(new IdentityVideoPipe(depacketizer.OutputFormat)));
            return new VideoTrack(depacketizer).Next(list.ToArray());
        }

        public bool Initialize()
        {
            return Initialize(null, null);
        }

        public bool Initialize(RtcAudioTrackConfig audioTrackConfig, RtcVideoTrackConfig videoTrackConfig)
        {
            lock (_InitializeLock)
            {
                if (_Initialized)
                {
                    return false;
                }

                _Initialized = true;
            }

            if (audioTrackConfig != null)
            {
                OpusDisabled = audioTrackConfig.OpusDisabled;
                G722Disabled = audioTrackConfig.G722Disabled;
                PcmuDisabled = audioTrackConfig.PcmuDisabled;
                PcmaDisabled = audioTrackConfig.PcmaDisabled;
            }

            if (videoTrackConfig != null)
            {
                Vp8Disabled = videoTrackConfig.Vp8Disabled;
                H264Disabled = videoTrackConfig.H264Disabled;
                Vp9Disabled = videoTrackConfig.Vp9Disabled;
                H265Disabled = videoTrackConfig.H265Disabled;
            }

            try
            {
                if (!AudioDisabled)
                {
                    AudioTrack audioTrack = new AudioTrack();
                    List<AudioTrack> list = new List<AudioTrack>();
                    if (!OpusDisabled)
                    {
                        try
                        {
                            OpusDecoder = CreateOpusDecoder(_OpusConfig);
                        }
                        catch (Exception ex)
                        {
                            //_log.Error("Could not create remote Opus decoder.", ex);
                        }

                        if (OpusDecoder != null)
                        {
                            OpusDepacketizer = CreateOpusDepacketizer(OpusDecoder.InputConfig);
                            if (AecDisabled)
                            {
                                OpusSink = CreateAudioSink(OpusDecoder.OutputConfig);
                            }
                            else
                            {
                                OpusConverter = CreateSoundConverter(AecContext.Config);
                                OpusSink = AecContext.OutputMixerSink;
                            }

                            if (OpusConverter == null)
                            {
                                OpusSynchronizer = CreateAudioSynchronizer(OpusDecoder.OutputFormat);
                            }
                            else
                            {
                                OpusSynchronizer = CreateAudioSynchronizer(OpusConverter.OutputFormat);
                            }

                            list.Add(CreateAudioTrack(OpusDepacketizer, OpusDecoder, OpusConverter, OpusSynchronizer, OpusSink));
                        }
                    }

                    //if (!G722Disabled)
                    //{
                    //    try
                    //    {
                    //        G722Decoder = CreateG722Decoder(_G722Config);
                    //    }
                    //    catch (Exception ex2)
                    //    {
                    //        //_log.Error("Could not create remote G.722 decoder.", ex2);
                    //    }

                    //    if (G722Decoder != null)
                    //    {
                    //        G722Depacketizer = CreateG722Depacketizer(G722Decoder.InputConfig);
                    //        if (AecDisabled)
                    //        {
                    //            G722Sink = CreateAudioSink(G722Decoder.OutputConfig);
                    //        }
                    //        else
                    //        {
                    //            G722Converter = CreateSoundConverter(AecContext.Config);
                    //            G722Sink = AecContext.OutputMixerSink;
                    //        }

                    //        if (G722Converter == null)
                    //        {
                    //            G722Synchronizer = CreateAudioSynchronizer(G722Decoder.OutputFormat);
                    //        }
                    //        else
                    //        {
                    //            G722Synchronizer = CreateAudioSynchronizer(G722Converter.OutputFormat);
                    //        }

                    //        list.Add(CreateAudioTrack(G722Depacketizer, G722Decoder, G722Converter, G722Synchronizer, G722Sink));
                    //    }
                    //}

                    //if (!PcmuDisabled)
                    //{
                    //    try
                    //    {
                    //        PcmuDecoder = CreatePcmuDecoder(_PcmuConfig);
                    //    }
                    //    catch (Exception ex3)
                    //    {
                    //        //_log.Error("Could not create remote PCMU decoder.", ex3);
                    //    }

                    //    if (PcmuDecoder != null)
                    //    {
                    //        PcmuDepacketizer = CreatePcmuDepacketizer(PcmuDecoder.InputConfig);
                    //        if (AecDisabled)
                    //        {
                    //            PcmuSink = CreateAudioSink(PcmuDecoder.OutputConfig);
                    //        }
                    //        else
                    //        {
                    //            PcmuConverter = CreateSoundConverter(AecContext.Config);
                    //            PcmuSink = AecContext.OutputMixerSink;
                    //        }

                    //        if (PcmuConverter == null)
                    //        {
                    //            PcmuSynchronizer = CreateAudioSynchronizer(PcmuDecoder.OutputFormat);
                    //        }
                    //        else
                    //        {
                    //            PcmuSynchronizer = CreateAudioSynchronizer(PcmuConverter.OutputFormat);
                    //        }

                    //        list.Add(CreateAudioTrack(PcmuDepacketizer, PcmuDecoder, PcmuConverter, PcmuSynchronizer, PcmuSink));
                    //    }
                    //}

                    //if (!PcmaDisabled)
                    //{
                    //    try
                    //    {
                    //        PcmaDecoder = CreatePcmaDecoder(_PcmaConfig);
                    //    }
                    //    catch (Exception ex4)
                    //    {
                    //        //_log.Error("Could not create remote PCMA decoder.", ex4);
                    //    }

                    //    if (PcmaDecoder != null)
                    //    {
                    //        PcmaDepacketizer = CreatePcmaDepacketizer(PcmaDecoder.InputConfig);
                    //        if (AecDisabled)
                    //        {
                    //            PcmaSink = CreateAudioSink(PcmaDecoder.OutputConfig);
                    //        }
                    //        else
                    //        {
                    //            PcmaConverter = CreateSoundConverter(AecContext.Config);
                    //            PcmaSink = AecContext.OutputMixerSink;
                    //        }

                    //        if (PcmaConverter == null)
                    //        {
                    //            PcmaSynchronizer = CreateAudioSynchronizer(PcmaDecoder.OutputFormat);
                    //        }
                    //        else
                    //        {
                    //            PcmaSynchronizer = CreateAudioSynchronizer(PcmaConverter.OutputFormat);
                    //        }

                    //        list.Add(CreateAudioTrack(PcmaDepacketizer, PcmaDecoder, PcmaConverter, PcmaSynchronizer, PcmaSink));
                    //    }
                    //}

                    if (list.Count > 0)
                    {
                        audioTrack = audioTrack.Next(list.ToArray());
                    }
                    else
                    {
                        //_log.Error("Could not initialize remote media. No audio decoders initialized. Check the logs for more detail.");
                    }

                    AddAudioTrack(audioTrack);
                }

                if (!VideoDisabled)
                {
                    VideoTrack videoTrack = new VideoTrack();
                    List<VideoTrack> list2 = new List<VideoTrack>();
                    try
                    {
                        ViewSink = CreateViewSink();
                    }
                    catch (Exception ex5)
                    {
                        //_log.Error("Could not create remote view sink.", ex5);
                    }

                    if (!Vp8Disabled)
                    {
                        try
                        {
                            Vp8Decoder = CreateVp8Decoder();
                        }
                        catch (Exception ex6)
                        {
                            //_log.Error("Could not create remote VP8 decoder.", ex6);
                        }

                        if (Vp8Decoder != null)
                        {
                            Vp8Depacketizer = CreateVp8Depacketizer();
                            if (ViewSink != null)
                            {
                                Vp8Converter = CreateImageConverter(ViewSink.InputFormat ?? Vp8Decoder.OutputFormat);
                            }

                            if (Vp8Converter == null)
                            {
                                Vp8Synchronizer = CreateVideoSynchronizer(Vp8Decoder.OutputFormat);
                            }
                            else
                            {
                                Vp8Synchronizer = CreateVideoSynchronizer(Vp8Converter.OutputFormat);
                            }

                            list2.Add(CreateVideoTrack(new VideoPipe[1]
                            {
                                Vp8Depacketizer
                            }, Vp8Decoder, Vp8Converter, Vp8Synchronizer));
                        }
                    }

                    if (!H264Disabled)
                    {
                        try
                        {
                            H264Decoders = (CreateH264Decoders() ?? new VideoDecoder[1]
                            {
                                CreateH264Decoder()
                            });
                        }
                        catch (Exception ex7)
                        {
                            //_log.Error("Could not create remote H.264 decoder.", ex7);
                        }

                        if (H264Decoders != null)
                        {
                            int num = H264Decoders.Length;
                            H264DepacketizersArray = new VideoPipe[num][];
                            H264Converters = new VideoPipe[num];
                            H264Synchronizers = new VideoSynchronizer[num];
                            for (int i = 0; i < num; i++)
                            {
                                if (H264Decoders[i] == null)
                                {
                                    continue;
                                }

                                H264DepacketizersArray[i] = (CreateH264Depacketizers() ?? new VideoPipe[1]
                                {
                                    CreateH264Depacketizer()
                                });
                                if (ViewSink != null)
                                {
                                    H264Converters[i] = CreateImageConverter(ViewSink.InputFormat ?? H264Decoders[i].OutputFormat);
                                }

                                if (H264Converters[i] == null)
                                {
                                    H264Synchronizers[i] = CreateVideoSynchronizer(H264Decoders[i].OutputFormat);
                                }
                                else
                                {
                                    H264Synchronizers[i] = CreateVideoSynchronizer(H264Converters[i].OutputFormat);
                                }

                                VideoFormat inputFormat = H264Decoders[i].InputFormat;
                                if (inputFormat != null)
                                {
                                    VideoPipe[] array = H264DepacketizersArray[i];
                                    foreach (VideoPipe obj in array)
                                    {
                                        VideoFormat inputFormat2 = obj.InputFormat;
                                        if (inputFormat2 != null)
                                        {
                                            inputFormat2.Profile = inputFormat.Profile;
                                            inputFormat2.Level = inputFormat.Level;
                                        }

                                        VideoFormat outputFormat = obj.OutputFormat;
                                        if (outputFormat != null)
                                        {
                                            outputFormat.Profile = inputFormat.Profile;
                                            outputFormat.Level = inputFormat.Level;
                                        }
                                    }
                                }

                                list2.Add(CreateVideoTrack(H264DepacketizersArray[i], H264Decoders[i], H264Converters[i], H264Synchronizers[i]));
                            }
                        }
                    }

                    if (!Vp9Disabled)
                    {
                        try
                        {
                            Vp9Decoder = CreateVp9Decoder();
                        }
                        catch (Exception ex8)
                        {
                            //_log.Error("Could not create remote VP9 decoder.", ex8);
                        }

                        if (Vp9Decoder != null)
                        {
                            Vp9Depacketizer = CreateVp9Depacketizer();
                            if (ViewSink != null)
                            {
                                Vp9Converter = CreateImageConverter(ViewSink.InputFormat ?? Vp9Decoder.OutputFormat);
                            }

                            if (Vp9Converter == null)
                            {
                                Vp9Synchronizer = CreateVideoSynchronizer(Vp9Decoder.OutputFormat);
                            }
                            else
                            {
                                Vp9Synchronizer = CreateVideoSynchronizer(Vp9Converter.OutputFormat);
                            }

                            list2.Add(CreateVideoTrack(new VideoPipe[1]
                            {
                                Vp9Depacketizer
                            }, Vp9Decoder, Vp9Converter, Vp9Synchronizer));
                        }
                    }

                    if (!H265Disabled)
                    {
                        try
                        {
                            H265Decoder = CreateH265Decoder();
                        }
                        catch (Exception ex9)
                        {
                            //_log.Error("Could not create remote H.265 decoder.", ex9);
                        }

                        if (H265Decoder != null)
                        {
                            H265Depacketizer = CreateH265Depacketizer();
                            if (ViewSink != null)
                            {
                                H265Converter = CreateImageConverter(ViewSink.InputFormat ?? H265Decoder.OutputFormat);
                            }

                            if (H265Converter == null)
                            {
                                H265Synchronizer = CreateVideoSynchronizer(H265Decoder.OutputFormat);
                            }
                            else
                            {
                                H265Synchronizer = CreateVideoSynchronizer(H265Converter.OutputFormat);
                            }

                            list2.Add(CreateVideoTrack(new VideoPipe[1]
                            {
                                H265Depacketizer
                            }, H265Decoder, H265Converter, H265Synchronizer));
                        }
                    }

                    if (list2.Count > 0)
                    {
                        videoTrack = videoTrack.Next(list2.ToArray());
                    }
                    else
                    {
                        //_log.Error("Could not initialize remote media. No video decoders initialized. Check the logs for more detail.");
                    }

                    if (ViewSink != null)
                    {
                        videoTrack = videoTrack.Next(ViewSink);
                    }

                    AddVideoTrack(videoTrack);
                }
            }
            catch (Exception ex10)
            {
                //_log.Error("Error occured while initializing remote media.", ex10);
                throw ex10;
            }

            return true;
        }

        protected abstract AudioSink CreateAudioSink(AudioConfig config);

        protected abstract ViewSink<TView> CreateViewSink();

        protected virtual AudioPipe CreateG722Depacketizer(AudioConfig config)
        {
            return new FM.LiveSwitch.G722.Depacketizer(config);
        }

        protected virtual AudioDecoder CreateG722Decoder(AudioConfig config)
        {
            return new FM.LiveSwitch.G722.Decoder(config);
        }

        protected virtual AudioPipe CreatePcmuDepacketizer(AudioConfig config)
        {
            return new FM.LiveSwitch.Pcmu.Depacketizer(config);
        }

        protected virtual AudioDecoder CreatePcmuDecoder(AudioConfig config)
        {
            return new FM.LiveSwitch.Pcmu.Decoder(config);
        }

        protected virtual AudioPipe CreatePcmaDepacketizer(AudioConfig config)
        {
            return new FM.LiveSwitch.Pcma.Depacketizer(config);
        }

        protected virtual AudioDecoder CreatePcmaDecoder(AudioConfig config)
        {
            return new FM.LiveSwitch.Pcma.Decoder(config);
        }

        protected virtual AudioPipe CreateOpusDepacketizer(AudioConfig config)
        {
            return new FM.LiveSwitch.Opus.Depacketizer(config);
        }

        protected abstract AudioDecoder CreateOpusDecoder(AudioConfig config);

        protected virtual VideoPipe CreateVp8Depacketizer()
        {
            return new FM.LiveSwitch.Vp8.Depacketizer();
        }

        protected abstract VideoDecoder CreateVp8Decoder();

        protected virtual VideoPipe CreateVp9Depacketizer()
        {
            return new FM.LiveSwitch.Vp9.Depacketizer();
        }

        protected abstract VideoDecoder CreateVp9Decoder();

        protected virtual VideoPipe CreateH264Depacketizer()
        {
            return new FM.LiveSwitch.H264.Depacketizer();
        }

        protected virtual VideoPipe[] CreateH264Depacketizers()
        {
            return null;
        }

        protected abstract VideoDecoder CreateH264Decoder();

        protected virtual VideoDecoder[] CreateH264Decoders()
        {
            return null;
        }

        protected virtual VideoPipe CreateH265Depacketizer()
        {
            return new FM.LiveSwitch.H265.Depacketizer();
        }

        protected virtual VideoDecoder CreateH265Decoder()
        {
            return null;
        }

        protected virtual AudioPipe CreateSoundConverter(AudioConfig config)
        {
            return new SoundConverter(config);
        }

        protected abstract VideoPipe CreateImageConverter(VideoFormat outputFormat);

        protected virtual AudioSynchronizer CreateAudioSynchronizer(AudioFormat format)
        {
            return new AudioSynchronizer(format);
        }

        protected virtual VideoSynchronizer CreateVideoSynchronizer(VideoFormat format)
        {
            return new VideoSynchronizer(format);
        }

        protected abstract AudioSink CreateAudioRecorder(AudioFormat inputFormat);

        protected abstract VideoSink CreateVideoRecorder(VideoFormat inputFormat);

        private AudioSink GetAudioRecorder(AudioFormat inputFormat)
        {
            AudioSink audioSink = CreateAudioRecorder(inputFormat);
            if (audioSink != null)
            {
                audioSink.Deactivated = true;
                _AudioRecorders.Add(audioSink);
            }

            return audioSink;
        }

        private VideoSink GetVideoRecorder(VideoFormat inputFormat)
        {
            VideoSink videoSink = CreateVideoRecorder(inputFormat);
            if (videoSink != null)
            {
                videoSink.Deactivated = true;
                _VideoRecorders.Add(videoSink);
            }

            return videoSink;
        }

        public bool ToggleAudioRecording()
        {
            lock (AudioRecordingLock)
            {
                base.IsRecordingAudio = !base.IsRecordingAudio;
                if (ViewSink != null)
                {
                    ViewSink.IsRecording = (base.IsRecordingVideo || base.IsRecordingAudio);
                }

                foreach (AudioSink audioRecorder in _AudioRecorders)
                {
                    audioRecorder.Deactivated = !base.IsRecordingAudio;
                }

                return base.IsRecordingAudio;
            }
        }

        public bool ToggleVideoRecording()
        {
            lock (VideoRecordingLock)
            {
                base.IsRecordingVideo = !base.IsRecordingVideo;
                if (ViewSink != null)
                {
                    ViewSink.IsRecording = (base.IsRecordingVideo || base.IsRecordingAudio);
                }

                foreach (VideoSink videoRecorder in _VideoRecorders)
                {
                    videoRecorder.Deactivated = !base.IsRecordingVideo;
                }

                return base.IsRecordingVideo;
            }
        }

        public override void Destroy()
        {
            if (!AecDisabled && base.AudioTrack != null)
            {
                IAudioOutput[] outputs = base.AudioTrack.Outputs;
                for (int i = 0; i < outputs.Length; i++)
                {
                    outputs[i].RemoveOutput(AecContext.OutputMixer);
                }
            }

            foreach (AudioSink audioRecorder in _AudioRecorders)
            {
                if (!audioRecorder.Persistent)
                {
                    audioRecorder.Destroy();
                }
            }

            foreach (VideoSink videoRecorder in _VideoRecorders)
            {
                if (!videoRecorder.Persistent)
                {
                    videoRecorder.Destroy();
                }
            }

            base.Destroy();
        }
    }
}

