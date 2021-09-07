using FM.LiveSwitch;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreamingLibrary
{
    public abstract class CustomInheritVideoSource : VideoSource
    {
        private VideoType _VideoType = VideoType.Unknown;

        private long _BaseSystemTimestamp = -1L;

        private long _BaseTimestamp = -1L;

        private int _LastRaiseWidth = -1;

        private int _LastRaiseHeight = -1;

        private double _MinOutputScale = -1.0;

        private double _MaxOutputScale = -1.0;

        private double _TargetOutputScale = -1.0;

        private double _MinOutputFrameRate = -1.0;

        private double _MaxOutputFrameRate = -1.0;

        private double _TargetOutputFrameRate = -1.0;

        private Size _MinOutputSize;

        private Size _MaxOutputSize;

        private Size _TargetOutputSize;

        public virtual VideoType VideoType
        {
            get
            {
                return _VideoType;
            }
            protected set
            {
                _VideoType = value;
            }
        }

        public long FrameCount
        {
            get;
            private set;
        }

        public int AverageFrameRate
        {
            get
            {
                if (FrameCount == 0L)
                {
                    return 0;
                }

                if (_BaseSystemTimestamp == -1)
                {
                    return 0;
                }

                long num = ManagedStopwatch.GetTimestamp() - _BaseSystemTimestamp;
                if (num < Constants.TicksPerSecond)
                {
                    return -1;
                }

                return (int)(FrameCount * Constants.TicksPerSecond / num);
            }
        }

        public override bool OverConstrainedOutput
        {
            get
            {
                if (!base.OverConstrainedOutput && !OverConstrainedOutputScale && !OverConstrainedOutputFrameRate)
                {
                    return OverConstrainedOutputSize;
                }

                return true;
            }
        }

        public virtual bool OverConstrainedScale => OverConstrainedOutputScale;

        public bool OverConstrainedOutputScale => ConstraintUtility.OverConstrained(MinOutputScale, MaxOutputScale);

        public virtual double MinOutputScale
        {
            get
            {
                return _MinOutputScale;
            }
            protected set
            {
                _MinOutputScale = value;
            }
        }

        public virtual double MaxOutputScale
        {
            get
            {
                return _MaxOutputScale;
            }
            protected set
            {
                _MaxOutputScale = value;
            }
        }

        public virtual double TargetOutputScale
        {
            get
            {
                return ConstraintUtility.ClampMin(_TargetOutputScale, MinOutputScale, MaxOutputScale);
            }
            protected set
            {
                _TargetOutputScale = value;
            }
        }

        public virtual bool OverConstrainedFrameRate => OverConstrainedOutputFrameRate;

        public bool OverConstrainedOutputFrameRate => ConstraintUtility.OverConstrained(MinOutputFrameRate, MaxOutputFrameRate);

        public virtual double MinOutputFrameRate
        {
            get
            {
                return _MinOutputFrameRate;
            }
            protected set
            {
                _MinOutputFrameRate = value;
            }
        }

        public virtual double MaxOutputFrameRate
        {
            get
            {
                return _MaxOutputFrameRate;
            }
            protected set
            {
                _MaxOutputFrameRate = value;
            }
        }

        public virtual double TargetOutputFrameRate
        {
            get
            {
                return ConstraintUtility.ClampMin(_TargetOutputFrameRate, MinOutputFrameRate, MaxOutputFrameRate);
            }
            protected set
            {
                _TargetOutputFrameRate = value;
            }
        }

        public virtual bool OverConstrainedSize => OverConstrainedOutputSize;

        public bool OverConstrainedOutputSize => ConstraintUtility.OverConstrained(MinOutputSize, MaxOutputSize);

        public virtual Size MinOutputSize
        {
            get
            {
                return _MinOutputSize;
            }
            protected set
            {
                _MinOutputSize = value;
            }
        }

        public virtual Size MaxOutputSize
        {
            get
            {
                return _MaxOutputSize;
            }
            protected set
            {
                _MaxOutputSize = value;
            }
        }

        public virtual Size TargetOutputSize
        {
            get
            {
                return ConstraintUtility.ClampMin(_TargetOutputSize, MinOutputSize, MaxOutputSize);
            }
            protected set
            {
                _TargetOutputSize = value;
            }
        }

        public virtual bool OverConstrainedWidth => OverConstrainedOutputWidth;

        public bool OverConstrainedOutputWidth => ConstraintUtility.OverConstrained(MinOutputWidth, MaxOutputWidth);

        public virtual int MinOutputWidth => ConstraintUtility.GetWidth(MinOutputSize);

        public virtual int MaxOutputWidth => ConstraintUtility.GetWidth(MaxOutputSize);

        public virtual int TargetOutputWidth => ConstraintUtility.GetWidth(TargetOutputSize);

        public virtual bool OverConstrainedHeight => OverConstrainedOutputHeight;

        public bool OverConstrainedOutputHeight => ConstraintUtility.OverConstrained(MinOutputHeight, MaxOutputHeight);

        public virtual int MinOutputHeight => ConstraintUtility.GetHeight(MinOutputSize);

        public virtual int MaxOutputHeight => ConstraintUtility.GetHeight(MaxOutputSize);

        public virtual int TargetOutputHeight => ConstraintUtility.GetHeight(TargetOutputSize);

        public override EncodingInfo MinOutputEncoding
        {
            get
            {
                EncodingInfo minOutputEncoding = base.MinOutputEncoding;
                if (minOutputEncoding != null)
                {
                    minOutputEncoding.Scale = MinOutputScale;
                    minOutputEncoding.FrameRate = MinOutputFrameRate;
                    minOutputEncoding.Size = MinOutputSize;
                }

                return minOutputEncoding;
            }
            protected set
            {
                base.MinOutputEncoding = value;
                if (value == null)
                {
                    MinOutputScale = -1.0;
                    MinOutputFrameRate = -1.0;
                    MinOutputSize = null;
                }
                else
                {
                    MinOutputScale = value.Scale;
                    MinOutputFrameRate = value.FrameRate;
                    MinOutputSize = value.Size;
                }
            }
        }

        public override EncodingInfo MaxOutputEncoding
        {
            get
            {
                EncodingInfo maxOutputEncoding = base.MaxOutputEncoding;
                if (maxOutputEncoding != null)
                {
                    maxOutputEncoding.Scale = MaxOutputScale;
                    maxOutputEncoding.FrameRate = MaxOutputFrameRate;
                    maxOutputEncoding.Size = MaxOutputSize;
                }

                return maxOutputEncoding;
            }
            protected set
            {
                base.MaxOutputEncoding = value;
                if (value == null)
                {
                    MaxOutputScale = -1.0;
                    MaxOutputFrameRate = -1.0;
                    MaxOutputSize = null;
                }
                else
                {
                    MaxOutputScale = value.Scale;
                    MaxOutputFrameRate = value.FrameRate;
                    MaxOutputSize = value.Size;
                }
            }
        }

        public override FM.LiveSwitch.EncodingInfo TargetOutputEncoding
        {
            get
            {
                FM.LiveSwitch.EncodingInfo targetOutputEncoding = base.TargetOutputEncoding;
                if (targetOutputEncoding != null)
                {
                    targetOutputEncoding.Scale = TargetOutputScale;
                    targetOutputEncoding.FrameRate = TargetOutputFrameRate;
                    targetOutputEncoding.Size = TargetOutputSize;
                }

                return targetOutputEncoding;
            }
            protected set
            {
                base.TargetOutputEncoding = value;
                if (value == null)
                {
                    TargetOutputScale = -1.0;
                    TargetOutputFrameRate = -1.0;
                    TargetOutputSize = null;
                }
                else
                {
                    TargetOutputScale = value.Scale;
                    TargetOutputFrameRate = value.FrameRate;
                    TargetOutputSize = value.Size;
                }
            }
        }

        public event Action1<Size> OnRaiseSizeChange;

        public CustomInheritVideoSource(VideoFormat outputFormat)
            : base(outputFormat)
        {
            base.OutputSynchronizable = true;
        }

        public int GetSizeDistance(int width1, int height1, int width2, int height2)
        {
            if (width2 > 0 && height2 > 0)
            {
                return MathAssistant.Abs(width2 - width1) + MathAssistant.Abs(height2 - height1);
            }

            if (width2 <= 0)
            {
                return MathAssistant.Abs(height2 - height1);
            }

            if (height2 <= 0)
            {
                return MathAssistant.Abs(width2 - width1);
            }

            return -1;
        }

        public double GetFrameRateDistance(double frameRate1, double frameRate2)
        {
            if (frameRate2 > 0.0)
            {
                return MathAssistant.Abs(frameRate2 - frameRate1);
            }

            return -1.0;
        }

        protected override IVideoInputCollection CreateInputCollection(IVideoOutput output)
        {
            return new IVideoInputCollection(output);
        }

        protected override void RaiseFrame(VideoFrame frame)
        {
            FrameCount++;
            Debug.Log($"raise frame from inherit method {FrameCount}");
            if (frame.Timestamp != -1 && frame.SystemTimestamp == -1)
            {
                if (_BaseSystemTimestamp == -1)
                {
                    _BaseSystemTimestamp = ManagedStopwatch.GetTimestamp();
                    _BaseTimestamp = frame.Timestamp;
                }

                frame.SystemTimestamp = MediaFrame<VideoBuffer, VideoBufferCollection, VideoFormat, VideoFrame>.CalculateSystemTimestamp(_BaseSystemTimestamp, frame.Timestamp, base.OutputFormat.ClockRate, _BaseTimestamp);
            }

            if (frame.SystemTimestamp != -1 && frame.Timestamp == -1)
            {
                if (_BaseSystemTimestamp == -1)
                {
                    _BaseSystemTimestamp = frame.SystemTimestamp;
                }

                frame.Timestamp = MediaFrame<VideoBuffer, VideoBufferCollection, VideoFormat, VideoFrame>.CalculateTimestamp(_BaseSystemTimestamp, frame.SystemTimestamp, base.OutputFormat.ClockRate);
            }

            if (frame.Timestamp == -1 && frame.SystemTimestamp == -1)
            {
                long timestamp = ManagedStopwatch.GetTimestamp();
                if (_BaseSystemTimestamp == -1)
                {
                    _BaseSystemTimestamp = timestamp;
                }

                frame.SystemTimestamp = timestamp;
                frame.Timestamp = MediaFrame<VideoBuffer, VideoBufferCollection, VideoFormat, VideoFrame>.CalculateTimestamp(_BaseSystemTimestamp, timestamp, base.OutputFormat.ClockRate);
            }

            VideoBuffer lastBuffer = frame.LastBuffer;
            if (lastBuffer != null)
            {
                int width = lastBuffer.Width;
                int height = lastBuffer.Height;
                if (width != _LastRaiseWidth || height != _LastRaiseHeight)
                {
                    this.OnRaiseSizeChange?.Invoke(new Size(width, height));
                    _LastRaiseWidth = width;
                    _LastRaiseHeight = height;
                }
            }

            base.RaiseFrame(frame);
        }

        protected override bool OutputCanProcessFrame(IVideoInput output)
        {
            if (!base.OutputCanProcessFrame(output))
            {
                return false;
            }

            double maxOutputScale = MaxOutputScale;
            double minInputScale = output.MinInputScale;
            if (minInputScale != -1.0 && maxOutputScale != -1.0 && minInputScale > maxOutputScale)
            {
                return false;
            }

            double minOutputScale = MinOutputScale;
            double maxInputScale = output.MaxInputScale;
            if (maxInputScale != -1.0 && minOutputScale != -1.0 && maxInputScale < minOutputScale)
            {
                return false;
            }

            double maxOutputFrameRate = MaxOutputFrameRate;
            double minInputFrameRate = output.MinInputFrameRate;
            if (minInputFrameRate != -1.0 && maxOutputFrameRate != -1.0 && minInputFrameRate > maxOutputFrameRate)
            {
                return false;
            }

            double minOutputFrameRate = MinOutputFrameRate;
            double maxInputFrameRate = output.MaxInputFrameRate;
            if (maxInputFrameRate != -1.0 && minOutputFrameRate != -1.0 && maxInputFrameRate < minOutputFrameRate)
            {
                return false;
            }

            Size maxOutputSize = MaxOutputSize;
            Size minInputSize = output.MinInputSize;
            if (minInputSize != null && maxOutputSize != null)
            {
                if (minInputSize.Width != -1 && maxOutputSize.Width != -1 && minInputSize.Width > maxOutputSize.Width)
                {
                    return false;
                }

                if (minInputSize.Height != -1 && maxOutputSize.Height != -1 && minInputSize.Height > maxOutputSize.Height)
                {
                    return false;
                }
            }

            Size minOutputSize = MinOutputSize;
            Size maxInputSize = output.MaxInputSize;
            if (maxInputSize != null && minOutputSize != null)
            {
                if (maxInputSize.Width != -1 && minOutputSize.Width != -1 && maxInputSize.Width < minOutputSize.Width)
                {
                    return false;
                }

                if (maxInputSize.Height != -1 && minOutputSize.Height != -1 && maxInputSize.Height < minOutputSize.Height)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
