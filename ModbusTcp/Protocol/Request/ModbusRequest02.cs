using System;
using System.Net;
using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol.Request
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class ModbusRequest02 : ModbusBase
    {
        public ModbusRequest02(byte Unit = 0x01)
        {
            FunctionCode = 0x02;
            UnitIdentifier = Unit;
        }

        public ModbusRequest02(int offset, int numberOfInputs, byte Unit = 0x01)
            : this()
        {
            ReferenceNumber = (short)offset;
            BitCount = (short)numberOfInputs;
        }

        [MarshalAs(UnmanagedType.U1)]
        public byte UnitIdentifier;

        [MarshalAs(UnmanagedType.U1)]
        public byte FunctionCode;

        [MarshalAs(UnmanagedType.U2)]
        public short ReferenceNumber;

        [MarshalAs(UnmanagedType.U2)]
        public short BitCount;

        public override byte[] ToNetworkBuffer()
        {
            var copy = (ModbusRequest02)MemberwiseClone();
            copy.Header = Header.Clone();

            copy.Header.Length = IPAddress.HostToNetworkOrder(Header.Length);
            copy.Header.ProtocolIdentifier = IPAddress.HostToNetworkOrder(Header.ProtocolIdentifier);
            copy.Header.TransactionIdentifier = IPAddress.HostToNetworkOrder(Header.TransactionIdentifier);

            copy.ReferenceNumber = IPAddress.HostToNetworkOrder(copy.ReferenceNumber);
            copy.BitCount = IPAddress.HostToNetworkOrder(copy.BitCount);

            return copy.ToNetworkBytes();
        }
    }
}
