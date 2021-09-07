using System;

internal static class ImageUtility
{
    public unsafe static void VerticalMirror(byte[] inputBytes, byte[] outputBytes, int width, int height, int stride)
    {
        int num = 0;
        int num2 = inputBytes.Length - stride;
        fixed (byte* ptr = inputBytes)
        {
            fixed (byte* ptr2 = outputBytes)
            {
                for (int i = 0; i < height / 2; i++)
                {
                    Buffer.MemoryCopy(ptr + num, ptr2 + num2, stride, stride);
                    Buffer.MemoryCopy(ptr + num2, ptr2 + num, stride, stride);
                    num += stride;
                    num2 -= stride;
                }
            }
        }
    }
}