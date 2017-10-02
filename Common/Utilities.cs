using System.Runtime.InteropServices;

namespace System
{
    static class Utilities
    {
        public static byte[] ToNetworkBytes(this object item)
        {
            var len = Marshal.SizeOf(item.GetType());
            var arr = new byte[len];

            var ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(item, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);

            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public static byte[] ToNetworkBytes(this float[] values)
        {
            var idx = 0;
            var output = new byte[values.Length * 4];
            for (int i = 0; i < values.Length; i++)
            {
                var buf = BitConverter.GetBytes(values[i]);
                Array.Reverse(buf);
                Buffer.BlockCopy(buf, 0, output, idx, buf.Length);
                idx += buf.Length;
            }

            return output;
        }

        public static byte[] ToNetworkBytes(this short[] values)
        {
            var idx = 0;
            var output = new byte[values.Length * 2];
            for (int i = 0; i < values.Length; i++)
            {
                var buf = BitConverter.GetBytes(values[i]);
                Array.Reverse(buf);
                Buffer.BlockCopy(buf, 0, output, idx, buf.Length);
                idx += buf.Length;
            }

            return output;
        }

    }
}
