using FM.LiveSwitch;

internal class ConstraintUtility
{
    public static bool OverConstrained(int minIntValue, int maxIntValue)
    {
        if (minIntValue < 0 || maxIntValue < 0)
        {
            return false;
        }

        return minIntValue > maxIntValue;
    }

    public static bool OverConstrained(long minLongValue, long maxLongValue)
    {
        if (minLongValue < 0 || maxLongValue < 0)
        {
            return false;
        }

        return minLongValue > maxLongValue;
    }

    public static bool OverConstrained(double minDoubleValue, double maxDoubleValue)
    {
        if (minDoubleValue < 0.0 || maxDoubleValue < 0.0)
        {
            return false;
        }

        return minDoubleValue > maxDoubleValue;
    }

    public static bool OverConstrained(Size minSize, Size maxSize)
    {
        if (minSize == null || maxSize == null)
        {
            return false;
        }

        if (minSize.Width >= 0 && maxSize.Width >= 0 && minSize.Width > maxSize.Width)
        {
            return true;
        }

        if (minSize.Height >= 0 && maxSize.Height >= 0 && minSize.Height > maxSize.Height)
        {
            return true;
        }

        return false;
    }

    public static int Min(int intValue1, int intValue2)
    {
        if (intValue1 < 0)
        {
            return intValue2;
        }

        if (intValue2 < 0)
        {
            return intValue1;
        }

        return MathAssistant.Min(intValue1, intValue2);
    }

    public static long Min(long longValue1, long longValue2)
    {
        if (longValue1 < 0)
        {
            return longValue2;
        }

        if (longValue2 < 0)
        {
            return longValue1;
        }

        return MathAssistant.Min(longValue1, longValue2);
    }

    public static double Min(double doubleValue1, double doubleValue2)
    {
        if (doubleValue1 < 0.0)
        {
            return doubleValue2;
        }

        if (doubleValue2 < 0.0)
        {
            return doubleValue1;
        }

        return MathAssistant.Min(doubleValue1, doubleValue2);
    }

    public static Size Min(Size size1, Size size2)
    {
        if (size1 == null)
        {
            return size2;
        }

        if (size2 == null)
        {
            return size1;
        }

        return new Size(Min(size1.Width, size2.Width), Min(size1.Height, size2.Height));
    }

    public static int Max(int intValue1, int intValue2)
    {
        if (intValue1 < 0)
        {
            return intValue2;
        }

        if (intValue2 < 0)
        {
            return intValue1;
        }

        return MathAssistant.Max(intValue1, intValue2);
    }

    public static long Max(long longValue1, long longValue2)
    {
        if (longValue1 < 0)
        {
            return longValue2;
        }

        if (longValue2 < 0)
        {
            return longValue1;
        }

        return MathAssistant.Max(longValue1, longValue2);
    }

    public static double Max(double doubleValue1, double doubleValue2)
    {
        if (doubleValue1 < 0.0)
        {
            return doubleValue2;
        }

        if (doubleValue2 < 0.0)
        {
            return doubleValue1;
        }

        return MathAssistant.Max(doubleValue1, doubleValue2);
    }

    public static Size Max(Size size1, Size size2)
    {
        if (size1 == null)
        {
            return size2;
        }

        if (size2 == null)
        {
            return size1;
        }

        return new Size(Max(size1.Width, size2.Width), Max(size1.Height, size2.Height));
    }

    public static int GetWidth(Size size)
    {
        if (size == null)
        {
            return -1;
        }

        return size.Width;
    }

    public static int GetHeight(Size size)
    {
        if (size == null)
        {
            return -1;
        }

        return size.Height;
    }

    public static int ClampMin(int intValue, int minIntValue, int maxIntValue)
    {
        return Clamp(intValue, minIntValue, maxIntValue, minPreference: true);
    }

    public static long ClampMin(long longValue, long minLongValue, long maxLongValue)
    {
        return Clamp(longValue, minLongValue, maxLongValue, minPreference: true);
    }

    public static double ClampMin(double doubleValue, double minDoubleValue, double maxDoubleValue)
    {
        return Clamp(doubleValue, minDoubleValue, maxDoubleValue, minPreference: true);
    }

    public static Size ClampMin(Size size, Size minSize, Size maxSize)
    {
        return Clamp(size, minSize, maxSize, minPreference: true);
    }

    public static int ClampMax(int intValue, int minIntValue, int maxIntValue)
    {
        return Clamp(intValue, minIntValue, maxIntValue, minPreference: false);
    }

    public static long ClampMax(long longValue, long minLongValue, long maxLongValue)
    {
        return Clamp(longValue, minLongValue, maxLongValue, minPreference: false);
    }

    public static double ClampMax(double doubleValue, double minDoubleValue, double maxDoubleValue)
    {
        return Clamp(doubleValue, minDoubleValue, maxDoubleValue, minPreference: false);
    }

    public static Size ClampMax(Size size, Size minSize, Size maxSize)
    {
        return Clamp(size, minSize, maxSize, minPreference: false);
    }

    private static int Clamp(int intValue, int minIntValue, int maxIntValue, bool minPreference)
    {
        if (minIntValue == -1 && maxIntValue == -1)
        {
            return intValue;
        }

        if (maxIntValue == -1)
        {
            if (intValue == -1)
            {
                return intValue;
            }

            return MathAssistant.Max(intValue, minIntValue);
        }

        if (minIntValue == -1)
        {
            if (intValue == -1)
            {
                return maxIntValue;
            }

            return MathAssistant.Min(intValue, maxIntValue);
        }

        if (intValue == -1)
        {
            if (!minPreference)
            {
                return maxIntValue;
            }

            return minIntValue;
        }

        return MathAssistant.Min(MathAssistant.Max(intValue, minIntValue), maxIntValue);
    }

    private static long Clamp(long longValue, long minLongValue, long maxLongValue, bool minPreference)
    {
        if (minLongValue == -1 && maxLongValue == -1)
        {
            return longValue;
        }

        if (maxLongValue == -1)
        {
            if (longValue == -1)
            {
                return longValue;
            }

            return MathAssistant.Max(longValue, minLongValue);
        }

        if (minLongValue == -1)
        {
            if (longValue == -1)
            {
                return maxLongValue;
            }

            return MathAssistant.Min(longValue, maxLongValue);
        }

        if (longValue == -1)
        {
            if (!minPreference)
            {
                return maxLongValue;
            }

            return minLongValue;
        }

        return MathAssistant.Min(MathAssistant.Max(longValue, minLongValue), maxLongValue);
    }

    private static double Clamp(double doubleValue, double minDoubleValue, double maxDoubleValue, bool minPreference)
    {
        if (minDoubleValue == -1.0 && maxDoubleValue == -1.0)
        {
            return doubleValue;
        }

        if (maxDoubleValue == -1.0)
        {
            if (doubleValue == -1.0)
            {
                return doubleValue;
            }

            return MathAssistant.Max(doubleValue, minDoubleValue);
        }

        if (minDoubleValue == -1.0)
        {
            if (doubleValue == -1.0)
            {
                return maxDoubleValue;
            }

            return MathAssistant.Min(doubleValue, maxDoubleValue);
        }

        if (doubleValue == -1.0)
        {
            if (!minPreference)
            {
                return maxDoubleValue;
            }

            return minDoubleValue;
        }

        return MathAssistant.Min(MathAssistant.Max(doubleValue, minDoubleValue), maxDoubleValue);
    }

    private static Size Clamp(Size size, Size minSize, Size maxSize, bool minPreference)
    {
        if (minSize == null && maxSize == null)
        {
            return size;
        }

        if (maxSize == null)
        {
            if (size == null)
            {
                return size;
            }

            return new Size(Max(size.Width, minSize.Width), Max(size.Height, minSize.Height));
        }

        if (minSize == null)
        {
            if (size == null)
            {
                return maxSize;
            }

            return new Size(Min(size.Width, maxSize.Width), Min(size.Height, maxSize.Height));
        }

        if (size == null)
        {
            if (!minPreference)
            {
                return maxSize;
            }

            return minSize;
        }

        return new Size(Min(Max(size.Width, minSize.Width), maxSize.Width), Min(Max(size.Height, minSize.Height), maxSize.Height));
    }

    public static int Coalesce(int intValue1, int intValue2)
    {
        if (intValue1 != -1)
        {
            return intValue1;
        }

        return intValue2;
    }

    public static long Coalesce(long longValue1, long longValue2)
    {
        if (longValue1 != -1)
        {
            return longValue1;
        }

        return longValue2;
    }

    public static double Coalesce(double doubleValue1, double doubleValue2)
    {
        if (doubleValue1 != -1.0)
        {
            return doubleValue1;
        }

        return doubleValue2;
    }
}