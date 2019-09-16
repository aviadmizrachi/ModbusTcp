using System.Net;
using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ModbusHeader
    {
        [MarshalAs(UnmanagedType.U2)]
        public short TransactionIdentifier;

        [MarshalAs(UnmanagedType.U2)]
        public short ProtocolIdentifier;

        [MarshalAs(UnmanagedType.U2)]
        public short Length = FixedLength;

        public const int FixedLength = 6;

        public ModbusHeader Clone()
        {
            return (ModbusHeader)MemberwiseClone();
        }

        public byte[] ToNetworkBuffer()
        {
            Length = IPAddress.HostToNetworkOrder(Length);
            TransactionIdentifier = IPAddress.HostToNetworkOrder(Length);
            Length = IPAddress.HostToNetworkOrder(Length);

            return ToNetworkBuffer();
        }

        public static ModbusHeader FromNetworkBuffer(byte[] buffer)
        {
            var header = new ModbusHeader();

            int size = Marshal.SizeOf(header);
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(buffer, 0, ptr, size);

            header = (ModbusHeader)Marshal.PtrToStructure(ptr, header.GetType());
            Marshal.FreeHGlobal(ptr);

            header.Length = IPAddress.NetworkToHostOrder(header.Length);
            header.ProtocolIdentifier = IPAddress.NetworkToHostOrder(header.ProtocolIdentifier);
            header.TransactionIdentifier = IPAddress.NetworkToHostOrder(header.TransactionIdentifier);

            return header;
        }
    }
}
