using System;
using System.Net;
using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol.Request
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class ModbusRequest03 : ModbusRequestBase
    {
        public ModbusRequest03(byte Unit = 0x01)
        {
            FunctionCode = 0x03;
            UnitIdentifier = Unit;
        }

        public ModbusRequest03(int offset, int numberOfWords, byte Unit = 0x01)
            : this(Unit)
        {
            ReferenceNumber = (short)offset;
            WordCount = (short)numberOfWords;
        }

        [MarshalAs(UnmanagedType.U2)]
        public short WordCount;

        public override byte[] ToNetworkBuffer()
        {
            var copy = (ModbusRequest03)MemberwiseClone();
            copy.Header = Header.Clone();
            copy.ApplyNetworkOrderForBase();

            copy.WordCount = IPAddress.HostToNetworkOrder(copy.WordCount);

            return copy.ToNetworkBytes();
        }
    }
}
