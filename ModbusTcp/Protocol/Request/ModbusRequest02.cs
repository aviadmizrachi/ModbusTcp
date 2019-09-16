using System;
using System.Net;
using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol.Request
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class ModbusRequest02 : ModbusRequestBase
    {
        public ModbusRequest02(byte Unit = 0x01)
        {
            FunctionCode = 0x02;
            UnitIdentifier = Unit;
        }

        public ModbusRequest02(int offset, int numberOfInputs, byte Unit = 0x01)
            : this(Unit)
        {
            ReferenceNumber = (short)offset;
            BitCount = (short)numberOfInputs;
        }

        [MarshalAs(UnmanagedType.U2)]
        public short BitCount;

        public override byte[] ToNetworkBuffer()
        {
            var copy = (ModbusRequest02)MemberwiseClone();
            copy.Header = Header.Clone();
            copy.ApplyNetworkOrderForBase();

            copy.BitCount = IPAddress.HostToNetworkOrder(copy.BitCount);

            return copy.ToNetworkBytes();
        }
    }
}
