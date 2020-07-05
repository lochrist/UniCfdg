using System;
using UnityEngine;

public class Steganography
{
    public enum Format
    {
        RGB1,
        RGB2,
        RGBA1,
        RGBA2,
        A1,
        A2
    }

    public static byte[] kHeader = System.Text.Encoding.ASCII.GetBytes("#ST");
    public const int kHeaderSize = 8;
    public const int kHeaderNbPixels = 8;

    public static Texture2D Encode(Texture2D src, string msg, Format format = Format.RGBA2)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(msg);
        return Encode(src, bytes, format);
    }

    public static Texture2D Encode(Texture2D src, byte[] bytes, Format format = Format.RGBA2)
    {
        var maxSize = TextureMaxDataSize(src, format);
        if (maxSize < bytes.Length)
        {
            throw new System.Exception(string.Format("Cannot encode, data is too big: {0} vs {1}", bytes.Length, maxSize));
        }

        var pixels = src.GetPixels32();
        var copy = new Texture2D(src.width, src.height, src.format, false);
        var header = new byte[kHeaderSize];
        kHeader.CopyTo(header, 0);
        header[3] = (byte)format;
        var srcSize = System.BitConverter.GetBytes(bytes.Length);
        srcSize.CopyTo(header, 4);
        EncodeRGBA2(header, pixels, 0);
        switch (format)
        {
            case Format.A1:
                EncodeA1(bytes, pixels, kHeaderNbPixels);
                break;
            case Format.A2:
                EncodeA2(bytes, pixels, kHeaderNbPixels);
                break;
            case Format.RGB1:
                EncodeRGB1(bytes, pixels, kHeaderNbPixels);
                break;
            case Format.RGB2:
                EncodeRGB2(bytes, pixels, kHeaderNbPixels);
                break;
            case Format.RGBA1:
                EncodeRGBA1(bytes, pixels, kHeaderNbPixels);
                break;
            case Format.RGBA2:
                EncodeRGBA2(bytes, pixels, kHeaderNbPixels);
                break;
        }
        copy.SetPixels32(pixels);
        copy.Apply();

        return copy;
    }

    public static int TextureMaxDataSize(Texture2D src, Format format = Format.RGBA2)
    {
        var nbDataPixels = (src.width * src.height) - kHeaderNbPixels;
        if (nbDataPixels <= 0)
            return 0;
        var bitCapacity = 0;
        switch (format)
        {
            case Format.A1:
                bitCapacity = nbDataPixels;
                break;
            case Format.A2:
                bitCapacity = nbDataPixels * 2;
                break;
            case Format.RGB1:
                bitCapacity = nbDataPixels * 3;
                break;
            case Format.RGB2:
                bitCapacity = nbDataPixels * 6;
                break;
            case Format.RGBA1:
                bitCapacity = nbDataPixels * 4;
                break;
            case Format.RGBA2:
                bitCapacity = nbDataPixels * 8;
                break;
        }

        return (bitCapacity / 8);
    }

    public static byte[] Decode(Texture2D src)
    {
        var pixels = src.GetPixels32();
        if (pixels.Length < kHeaderNbPixels)
        {
            throw new System.Exception(string.Format("Cannot decode, src is too small"));
        }

        var header = new byte[kHeaderSize];
        var srcIndex = DecodeRGBA2(pixels, 0, header);
        if (header[0] != kHeader[0] || header[1] != kHeader[1] || header[2] != kHeader[2])
        {
            throw new Exception("Not an encoded image (header doesn't match)");
        }

        var format = (Format)header[3];
        var encodedDataSize = BitConverter.ToInt32(header, 4);
        var maxSize = TextureMaxDataSize(src, format);
        if (maxSize < encodedDataSize)
        {
            throw new System.Exception(string.Format("Cannot decode, src is too small: {0} vs {1}", maxSize, encodedDataSize));
        }

        var dst = new byte[encodedDataSize];
        switch (format)
        {
            case Format.A1:
                DecodeA1(pixels, srcIndex, dst);
                break;
            case Format.A2:
                DecodeA2(pixels, srcIndex, dst);
                break;
            case Format.RGB1:
                DecodeRGB1(pixels, srcIndex, dst);
                break;
            case Format.RGB2:
                DecodeRGB2(pixels, srcIndex, dst);
                break;
            case Format.RGBA1:
                DecodeRGBA1(pixels, srcIndex, dst);
                break;
            case Format.RGBA2:
                DecodeRGBA2(pixels, srcIndex, dst);
                break;
        }

        return dst;
    }

    public static string DecodeAsString(Texture2D src)
    {
        return new string(System.Text.Encoding.UTF8.GetChars(Decode(src)));
    }

    private static int DecodeA2(Color32[] src, int srcIndex, byte[] dst)
    {
        for (var dstIndex = 0; dstIndex < dst.Length; dstIndex++)
        {
            dst[dstIndex] |= (byte)(src[srcIndex++].a & 0x03);
            dst[dstIndex] |= (byte)((src[srcIndex++].a & 0x03) << 2);
            dst[dstIndex] |= (byte)((src[srcIndex++].a & 0x03) << 4);
            dst[dstIndex] |= (byte)((src[srcIndex++].a & 0x03) << 6);
        }

        return srcIndex;
    }

    private static int DecodeRGB1(Color32[] src, int srcIndex, byte[] dst)
    {
        for (var dstIndex = 0; dstIndex < dst.Length; dstIndex++)
        {
            switch (dstIndex % 3)
            {
                case 0:
                    dst[dstIndex] |= (byte)(src[srcIndex].r & 0x01);
                    dst[dstIndex] |= (byte)((src[srcIndex].g & 0x01) << 1);
                    dst[dstIndex] |= (byte)((src[srcIndex].b & 0x01) << 2);
                    srcIndex++;
                    dst[dstIndex] |= (byte)((src[srcIndex].r & 0x01) << 3);
                    dst[dstIndex] |= (byte)((src[srcIndex].g & 0x01) << 4);
                    dst[dstIndex] |= (byte)((src[srcIndex].b & 0x01) << 5);
                    srcIndex++;
                    dst[dstIndex] |= (byte)((src[srcIndex].r & 0x01) << 6);
                    dst[dstIndex] |= (byte)((src[srcIndex].g & 0x01) << 7);
                    break;
                case 1:
                    dst[dstIndex] |= (byte)(src[srcIndex].b & 0x01);
                    srcIndex++;
                    dst[dstIndex] |= (byte)((src[srcIndex].r & 0x01) << 1);
                    dst[dstIndex] |= (byte)((src[srcIndex].g & 0x01) << 2);
                    dst[dstIndex] |= (byte)((src[srcIndex].b & 0x01) << 3);
                    srcIndex++;
                    dst[dstIndex] |= (byte)((src[srcIndex].r & 0x01) << 4);
                    dst[dstIndex] |= (byte)((src[srcIndex].g & 0x01) << 5);
                    dst[dstIndex] |= (byte)((src[srcIndex].b & 0x01) << 6);
                    srcIndex++;
                    dst[dstIndex] |= (byte)((src[srcIndex].r & 0x01) << 7);
                    break;
                case 2:
                    dst[dstIndex] |= (byte)(src[srcIndex].g & 0x01);
                    dst[dstIndex] |= (byte)((src[srcIndex].b & 0x01) << 1);
                    srcIndex++;
                    dst[dstIndex] |= (byte)((src[srcIndex].r & 0x01) << 2);
                    dst[dstIndex] |= (byte)((src[srcIndex].g & 0x01) << 3);
                    dst[dstIndex] |= (byte)((src[srcIndex].b & 0x01) << 4);
                    srcIndex++;
                    dst[dstIndex] |= (byte)((src[srcIndex].r & 0x01) << 5);
                    dst[dstIndex] |= (byte)((src[srcIndex].g & 0x01) << 6);
                    dst[dstIndex] |= (byte)((src[srcIndex].b & 0x01) << 7);
                    srcIndex++;
                    break;
            }
        }

        return srcIndex;
    }

    private static int DecodeRGB2(Color32[] src, int srcIndex, byte[] dst)
    {
        for (var dstIndex = 0; dstIndex < dst.Length; dstIndex++)
        {
            switch (dstIndex % 3)
            {
                case 0:
                    dst[dstIndex] |= (byte)(src[srcIndex].r & 0x03);
                    dst[dstIndex] |= (byte)((src[srcIndex].g & 0x03) << 2);
                    dst[dstIndex] |= (byte)((src[srcIndex].b & 0x03) << 4);
                    srcIndex++;
                    dst[dstIndex] |= (byte)((src[srcIndex].r & 0x03) << 6);
                    break;
                case 1:
                    dst[dstIndex] |= (byte)(src[srcIndex].g & 0x03);
                    dst[dstIndex] |= (byte)((src[srcIndex].b & 0x03) << 2);
                    srcIndex++;
                    dst[dstIndex] |= (byte)((src[srcIndex].r & 0x03) << 4);
                    dst[dstIndex] |= (byte)((src[srcIndex].g & 0x03) << 6);
                    break;
                case 2:
                    dst[dstIndex] |= (byte)(src[srcIndex].b & 0x03);
                    srcIndex++;
                    dst[dstIndex] |= (byte)((src[srcIndex].r & 0x03) << 2);
                    dst[dstIndex] |= (byte)((src[srcIndex].g & 0x03) << 4);
                    dst[dstIndex] |= (byte)((src[srcIndex].b & 0x03) << 6);
                    srcIndex++;
                    break;
            }
        }

        return srcIndex;
    }

    private static int DecodeA1(Color32[] src, int srcIndex, byte[] dst)
    {
        for (var dstIndex = 0; dstIndex < dst.Length; dstIndex++)
        {
            dst[dstIndex] |= (byte)(src[srcIndex++].a & 0x01);
            dst[dstIndex] |= (byte)((src[srcIndex++].a & 0x01) << 1);
            dst[dstIndex] |= (byte)((src[srcIndex++].a & 0x01) << 2);
            dst[dstIndex] |= (byte)((src[srcIndex++].a & 0x01) << 3);
            dst[dstIndex] |= (byte)((src[srcIndex++].a & 0x01) << 4);
            dst[dstIndex] |= (byte)((src[srcIndex++].a & 0x01) << 5);
            dst[dstIndex] |= (byte)((src[srcIndex++].a & 0x01) << 6);
            dst[dstIndex] |= (byte)((src[srcIndex++].a & 0x01) << 7);
        }

        return srcIndex;
    }

    private static int DecodeRGBA1(Color32[] src, int srcIndex, byte[] dst)
    {
        for (var dstIndex = 0; dstIndex < dst.Length; dstIndex++)
        {
            dst[dstIndex] |= (byte)(src[srcIndex].r & 0x01);
            dst[dstIndex] |= (byte)((src[srcIndex].g & 0x01) << 1);
            dst[dstIndex] |= (byte)((src[srcIndex].b & 0x01) << 2);
            dst[dstIndex] |= (byte)((src[srcIndex].a & 0x01) << 3);
            srcIndex++;

            dst[dstIndex] |= (byte)((src[srcIndex].r & 0x01) << 4);
            dst[dstIndex] |= (byte)((src[srcIndex].g & 0x01) << 5);
            dst[dstIndex] |= (byte)((src[srcIndex].b & 0x01) << 6);
            dst[dstIndex] |= (byte)((src[srcIndex].a & 0x01) << 7);
            srcIndex++;
        }

        return srcIndex;
    }

    private static int DecodeRGBA2(Color32[] src, int srcIndex, byte[] dst)
    {
        for (var dstIndex = 0; dstIndex < dst.Length; dstIndex++)
        {
            dst[dstIndex] |= (byte)(src[srcIndex].r & 0x03);
            dst[dstIndex] |= (byte)((src[srcIndex].g & 0x03) << 2);
            dst[dstIndex] |= (byte)((src[srcIndex].b & 0x03) << 4);
            dst[dstIndex] |= (byte)((src[srcIndex].a & 0x03) << 6);
            srcIndex++;
        }

        return srcIndex;
    }

    // Test if we need to copy the src pixels... or if we can modify them in po

    private static int EncodeA1(byte[] src, Color32[] dst, int dstIndex)
    {
        foreach (var b in src)
        {
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfe) | (b & 0x01));
            dstIndex++;
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfe) | ((b & 0x02) >> 0x01));
            dstIndex++;
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfe) | ((b & 0x04) >> 0x02));
            dstIndex++;
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfe) | ((b & 0x08) >> 0x03));
            dstIndex++;
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfe) | ((b & 0x10) >> 0x04));
            dstIndex++;
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfe) | ((b & 0x20) >> 0x05));
            dstIndex++;
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfe) | ((b & 0x40) >> 0x06));
            dstIndex++;
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfe) | ((b & 0x80) >> 0x07));
            dstIndex++;
        }
        return dstIndex;
    }

    private static void EncodeA2(byte[] src, Color32[] dst, int dstIndex)
    {
        foreach (var b in src)
        {
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfc) | (b & 0x03));
            ++dstIndex;
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfc) | ((b & 0x0C) >> 0x02));
            ++dstIndex;
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfc) | ((b & 0x30) >> 0x04));
            ++dstIndex;
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfc) | ((b & 0xc0) >> 0x06));
            ++dstIndex;
        }
    }

    private static int EncodeRGB1(byte[] src, Color32[] dst, int dstIndex)
    {
        for (var i = 0; i < src.Length; ++i)
        {
            var b = src[i];

            switch (i % 3)
            {
                case 0:
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfe) | (b & 0x01));
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfe) | ((b & 0x02) >> 0x01));
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfe) | ((b & 0x04) >> 0x02));
                    dstIndex++;
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfe) | ((b & 0x08) >> 0x03));
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfe) | ((b & 0x10) >> 0x04));
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfe) | ((b & 0x20) >> 0x05));
                    dstIndex++;
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfe) | ((b & 0x40) >> 0x06));
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfe) | ((b & 0x80) >> 0x07));
                    break;
                case 1:
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfe) | (b & 0x01));
                    dstIndex++;
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfe) | ((b & 0x02) >> 0x01));
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfe) | ((b & 0x04) >> 0x02));
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfe) | ((b & 0x08) >> 0x03));
                    dstIndex++;
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfe) | ((b & 0x10) >> 0x04));
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfe) | ((b & 0x20) >> 0x05));
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfe) | ((b & 0x40) >> 0x06));
                    dstIndex++;
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfe) | ((b & 0x80) >> 0x07));
                    break;
                case 2:
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfe) | (b & 0x01));
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfe) | ((b & 0x02) >> 0x01));
                    dstIndex++;
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfe) | ((b & 0x04) >> 0x02));
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfe) | ((b & 0x08) >> 0x03));
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfe) | ((b & 0x10) >> 0x04));
                    dstIndex++;
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfe) | ((b & 0x20) >> 0x05));
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfe) | ((b & 0x40) >> 0x06));
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfe) | ((b & 0x80) >> 0x07));
                    dstIndex++;
                    break;
            }
        }
        return dstIndex;
    }

    private static void EncodeRGB2(byte[] src, Color32[] dst, int dstIndex)
    {
        for (var i = 0; i < src.Length; ++i)
        {
            var b = src[i];
            switch (i % 3)
            {
                case 0:
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfc) | (b & 0x03));
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfc) | ((b & 0x0C) >> 0x02));
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfc) | ((b & 0x30) >> 0x04));
                    ++dstIndex;
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfc) | ((b & 0xc0) >> 0x06));
                    break;
                case 1:
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfc) | (b & 0x03));
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfc) | ((b & 0x0C) >> 0x02));
                    ++dstIndex;
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfc) | ((b & 0x30) >> 0x04));
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfc) | ((b & 0xc0) >> 0x06));
                    break;
                case 2:
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfc) | (b & 0x03));
                    ++dstIndex;
                    dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfc) | ((b & 0x0C) >> 0x02));
                    dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfc) | ((b & 0x30) >> 0x04));
                    dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfc) | ((b & 0xc0) >> 0x06));
                    ++dstIndex;
                    break;
            }
        }
    }

    private static int EncodeRGBA1(byte[] src, Color32[] dst, int dstIndex)
    {
        foreach (var b in src)
        {
            dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfe) | (b & 0x01));
            dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfe) | ((b & 0x02) >> 0x01));
            dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfe) | ((b & 0x04) >> 0x02));
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfe) | ((b & 0x08) >> 0x03));
            ++dstIndex;
            dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfe) | ((b & 0x10) >> 0x04));
            dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfe) | ((b & 0x20) >> 0x05));
            dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfe) | ((b & 0x40) >> 0x06));
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfe) | ((b & 0x80) >> 0x07));
            ++dstIndex;
        }
        return dstIndex;
    }

    private static void EncodeRGBA2(byte[] src, Color32[] dst, int dstIndex)
    {
        foreach (var b in src)
        {
            dst[dstIndex].r = (byte)((dst[dstIndex].r & 0xfc) | (b & 0x03));
            dst[dstIndex].g = (byte)((dst[dstIndex].g & 0xfc) | ((b & 0x0C) >> 0x02));
            dst[dstIndex].b = (byte)((dst[dstIndex].b & 0xfc) | ((b & 0x30) >> 0x04));
            dst[dstIndex].a = (byte)((dst[dstIndex].a & 0xfc) | ((b & 0xc0) >> 0x06));
            ++dstIndex;
        }
    }
}