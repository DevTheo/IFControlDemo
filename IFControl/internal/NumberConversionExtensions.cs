using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCore.Internal
{
    internal static class NumberConversionExtensions
    {
        internal static int ToInt(this char[] arr)
        {
            return (arr.Length > 1) ? ((int) arr[0] * Char.MaxValue) + arr[1] : (int) arr[0];
        }
        internal static IEnumerable<int> ToIntEnumerable(this byte[] arr)
        {
            var vals = Math.Ceiling((double) arr.Length / 2);
            for (int i = 0; i < vals; i++)
                yield return (((i * 2) + 1 < arr.Length) ? ((int) arr[i * 2] * Byte.MaxValue) + arr[(i * 2) + 1] : (int) arr[i * 2]);

        }
        internal static int ToInt(this byte[] arr)
        {
            return BitConverter.ToInt32(arr, 0);
        }
        internal static IEnumerable<uint> ToUIntEnumerable(this byte[] arr)
        {
            var vals = Math.Ceiling((double) arr.Length / 2);
            for (int i = 0; i < vals; i++)
                yield return (((i * 2) + 1 < arr.Length) ? ((uint) arr[i * 2] * Byte.MaxValue) + arr[(i * 2) + 1] : (uint) arr[i * 2]);

        }
        internal static byte[] ToByteArray(this uint[] arr)
        {
            var vals = new byte[(arr.Length * 2)];
            for (int i = 0; i < arr.Length; i++)
            {
                var bindx = i * 2;
                var byts = BitConverter.GetBytes(arr[i]);
                if (byts.Length > 1)
                {
                    vals[bindx] = byts[0];
                    vals[bindx + 1] = byts[0];
                }
                else
                {
                    vals[bindx] = 0;
                    vals[bindx + 1] = byts[0];
                }
            }
            return vals;

        }
        internal static IEnumerable<int> ToIntArray(this char[] arr)
        {
            var vals = Math.Ceiling((double) arr.Length / 2);
            for (int i = 0; i < vals; i++)
                yield return (((i * 2) + 1 < arr.Length) ? ((int) arr[i * 2] * Char.MaxValue) + arr[(i * 2) + 1] : (int) arr[i * 2]);

        }
        internal static string FromUtf8Uint(this uint[] v)
        {
            var byts = v.Select(i => (byte) i).ToArray();
            return UTF8Encoding.UTF8.GetString(byts, 0, byts.Length);
        }
        internal static uint GetUIntFromBufferLoc(this byte[] buf, long offset)
        {
            return buf.GetUIntFromBufferLoc((int) offset);
        }

        internal static uint GetUIntFromBufferLoc(this byte[] buf, uint offset)
        {
            return buf.GetUIntFromBufferLoc((int) offset);
        }
        internal static uint GetUIntFromBufferLoc(this byte[] buf, int offset)
        {
            return (((uint) buf[offset + 0]) << 24) | (((uint) buf[offset + 1]) << 16) | ((uint) buf[offset + 2] << 8) | ((uint) buf[offset + 3]);
        }
        internal static ushort GetUShortFromBufferLoc(this byte[] buf, long offset)
        {
            return buf.GetUShortFromBufferLoc((int) offset);
        }
        internal static ushort GetUShortFromBufferLoc(this byte[] buf, uint offset)
        {
            return buf.GetUShortFromBufferLoc((int) offset);
        }
        internal static ushort GetUShortFromBufferLoc(this byte[] buf, int offset)
        {
            return (ushort) (((buf[offset + 0]) << 8) | (buf[offset + 1]));
        }
        internal static void StoreUInt(this byte[] buf, long offset, long val)
        {
            buf.StoreUInt((int) offset, (uint) val);
        }
        internal static void StoreUInt(this byte[] buf, uint offset, uint val)
        {
            buf.StoreUInt((int) offset, val);
        }
        internal static void StoreUInt(this byte[] buf, int offset, uint val)
        {

            buf[offset] = (byte) (val >> 24);
            buf[offset + 1] = (byte) (val >> 16);
            buf[offset + 2] = (byte) (val >> 8);
            buf[offset + 3] = (byte) (val);
        }
        internal static void StoreUShort(this byte[] buf, long offset, long val)
        {
            buf.StoreUShort((int) offset, (ushort) val);
        }

        internal static void StoreUShort(this byte[] buf, uint offset, ushort val)
        {
            buf.StoreUShort((int) offset, val);
        }
        internal static void StoreUShort(this byte[] buf, int offset, ushort val)
        {
            buf[offset] = (byte) (val >> 8);
            buf[offset + 1] = (byte) (val);
        }

    }
}
