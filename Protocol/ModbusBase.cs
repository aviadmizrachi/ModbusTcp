using System.Net;
using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public abstract class ModbusBase
    {
        //protected ModbusBase CloneForNetworkBuffer()
        //{
        //    var copy = (ModbusBase)MemberwiseClone();
        //    copy.Header = Header.Clone();

        //    copy.Header.Length = IPAddress.HostToNetworkOrder(copy.Header.Length);
        //    copy.Header.ProtocolIdentifier = IPAddress.HostToNetworkOrder(copy.Header.ProtocolIdentifier);
        //    copy.Header.TransactionIdentifier = IPAddress.HostToNetworkOrder(copy.Header.TransactionIdentifier);

        //    return copy;
        //}

        protected byte[] ConvertToByteArray()
        {
            var len = Marshal.SizeOf(GetType());
            var arr = new byte[len];

            var ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);

            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public abstract byte[] ToNetworkBuffer();
    }
}
