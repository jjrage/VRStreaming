using FM.LiveSwitch;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingLibrary
{
    public abstract class CustomScreenSourceBase : VideoSource
    {
        private Size _Empty = Size.Empty;

        private Size _MinSize;

        private Size _MaxSize;

        private Size _TargetSize;

        private Size _Size;

        private double _MinFrameRate = -1.0;

        private double _MaxFrameRate = -1.0;

        private double _TargetFrameRate = -1.0;

        private double _FrameRate = -1.0;

        private Point _TargetOrigin;

        private Point _Origin;

        public virtual Size MinSupportedSize => _Empty;

        public virtual Size MaxSupportedSize => null;

        public bool StaticOutputSize
        {
            get;
            set;
        }

        public override Size MinOutputSize
        {
            get
            {
                if (StaticOutputSize)
                {
                    return MinSupportedSize;
                }

                return MinSize;
            }
            protected set
            {
                MinSize = value;
            }
        }

        public virtual Size MinSize
        {
            get
            {
                return ConstraintUtility.Max(_MinSize, MinSupportedSize);
            }
            set
            {
                _MinSize = value;
            }
        }

        public override Size MaxOutputSize
        {
            get
            {
                if (StaticOutputSize)
                {
                    return MaxSupportedSize;
                }

                return MaxSize;
            }
            protected set
            {
                MaxSize = value;
            }
        }

        public virtual Size MaxSize
        {
            get
            {
                return ConstraintUtility.Min(_MaxSize, MaxSupportedSize);
            }
            set
            {
                _MaxSize = value;
            }
        }

        public override Size TargetOutputSize
        {
            get
            {
                return TargetSize;
            }
            protected set
            {
                TargetSize = value;
            }
        }

        public virtual Size TargetSize
        {
            get
            {
                return ConstraintUtility.ClampMin(_TargetSize, MinOutputSize, MaxOutputSize);
            }
            set
            {
                if (value == null)
                {
                    throw new Exception("Target size cannot be null.");
                }

                _TargetSize = value;
            }
        }

        public virtual Size Size
        {
            get
            {
                return ConstraintUtility.ClampMin(_Size ?? _TargetSize, MinSupportedSize, MaxSupportedSize);
            }
            protected set
            {
                _Size = value;
            }
        }

        public virtual double MinSupportedFrameRate => 0.0;

        public virtual double MaxSupportedFrameRate => -1.0;

        public bool StaticOutputFrameRate
        {
            get;
            set;
        }

        public override double MinOutputFrameRate
        {
            get
            {
                if (StaticOutputFrameRate)
                {
                    return MinSupportedFrameRate;
                }

                return MinFrameRate;
            }
            protected set
            {
                MinFrameRate = value;
            }
        }

        public virtual double MinFrameRate
        {
            get
            {
                return ConstraintUtility.Max(_MinFrameRate, MinSupportedFrameRate);
            }
            set
            {
                _MinFrameRate = value;
            }
        }

        public override double MaxOutputFrameRate
        {
            get
            {
                if (StaticOutputFrameRate)
                {
                    return MaxSupportedFrameRate;
                }

                return MaxFrameRate;
            }
            protected set
            {
                MaxFrameRate = value;
            }
        }

        public virtual double MaxFrameRate
        {
            get
            {
                return ConstraintUtility.Min(_MaxFrameRate, MaxSupportedFrameRate);
            }
            set
            {
                _MaxFrameRate = value;
            }
        }

        public override double TargetOutputFrameRate
        {
            get
            {
                return TargetFrameRate;
            }
            protected set
            {
                TargetFrameRate = value;
            }
        }

        public virtual double TargetFrameRate
        {
            get
            {
                return ConstraintUtility.ClampMin(_TargetFrameRate, MinSupportedFrameRate, MaxSupportedFrameRate);
            }
            protected set
            {
                if (value <= 0.0)
                {
                    throw new Exception("Target frame-rate must be a positive number.");
                }

                _TargetFrameRate = value;
            }
        }

        public virtual double FrameRate
        {
            get
            {
                return ConstraintUtility.ClampMin(ConstraintUtility.Coalesce(_FrameRate, _TargetFrameRate), MinSupportedFrameRate, MaxSupportedFrameRate);
            }
            protected set
            {
                _FrameRate = value;
            }
        }

        public virtual Point TargetOrigin
        {
            get
            {
                return _TargetOrigin;
            }
            set
            {
                if (value == null)
                {
                    throw new Exception("Target origin cannot be null.");
                }

                _TargetOrigin = value;
            }
        }

        public virtual Point Origin
        {
            get
            {
                return _Origin ?? _TargetOrigin;
            }
            protected set
            {
                _Origin = value;
            }
        }

        public Rectangle TargetRegion
        {
            get
            {
                return new Rectangle(TargetOrigin, TargetSize);
            }
            set
            {
                if (value == null)
                {
                    throw new Exception("Target region cannot be null.");
                }

                TargetOrigin = value.Origin;
                TargetSize = value.Size;
            }
        }

        public Rectangle Region
        {
            get
            {
                return new Rectangle(Origin, Size);
            }
            protected set
            {
                if (value == null)
                {
                    Origin = null;
                    Size = null;
                }
                else
                {
                    Origin = value.Origin;
                    Size = value.Size;
                }
            }
        }

        public ScreenConfig TargetConfig
        {
            get
            {
                return new ScreenConfig(TargetRegion, TargetFrameRate);
            }
            set
            {
                if (value == null)
                {
                    throw new Exception("Target configuration cannot be null.");
                }

                TargetRegion = value.Region;
                TargetFrameRate = value.FrameRate;
            }
        }

        public ScreenConfig Config
        {
            get
            {
                return new ScreenConfig(Region, FrameRate);
            }
            protected set
            {
                if (value == null)
                {
                    Region = null;
                    FrameRate = -1.0;
                }
                else
                {
                    Region = value.Region;
                    FrameRate = value.FrameRate;
                }
            }
        }

        public CustomScreenSourceBase(VideoFormat outputFormat, ScreenConfig targetConfig)
            : base(outputFormat)
        {
            TargetConfig = targetConfig;
            VideoType = VideoType.Screen;
        }
    }
}

