using System;
using System.Net;
using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol.Request
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class ModbusRequest03 : ModbusBase
    {
        public ModbusRequest03()
        {
            FunctionCode = 0x03;
            UnitIdentifier = 0x01;
        }

        public ModbusRequest03(int offset, int numberOfWords)
            : this()
        {
            ReferenceNumber = (short)offset;
            WordCount = (short)numberOfWords;
        }

        [MarshalAs(UnmanagedType.U1)]
        public byte UnitIdentifier;

        [MarshalAs(UnmanagedType.U1)]
        public byte FunctionCode;

        [MarshalAs(UnmanagedType.U2)]
        public short ReferenceNumber;

        [MarshalAs(UnmanagedType.U2)]
        public short WordCount;

        public override byte[] ToNetworkBuffer()
        {
            var copy = (ModbusRequest03)MemberwiseClone();
            copy.Header = Header.Clone();

            copy.Header.Length = IPAddress.HostToNetworkOrder(Header.Length);
            copy.Header.ProtocolIdentifier = IPAddress.HostToNetworkOrder(Header.ProtocolIdentifier);
            copy.Header.TransactionIdentifier = IPAddress.HostToNetworkOrder(Header.TransactionIdentifier);

            copy.ReferenceNumber = IPAddress.HostToNetworkOrder(copy.ReferenceNumber);
            copy.WordCount = IPAddress.HostToNetworkOrder(copy.WordCount);

            return copy.ToNetworkBytes();
        }
    }
}
