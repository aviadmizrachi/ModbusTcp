using System;
using System.Net;
using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol.Request
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ModbusRequest05 : ModbusRequestBase
    {
        public const short FixedLength = 6;
        public static readonly short HighValue = BitConverter.ToInt16(new byte[] { 0xFF, 0x00 }, 0);
        public static readonly short LowValue = BitConverter.ToInt16(new byte[] { 0x00, 0x00 }, 0);

        public ModbusRequest05(byte Unit = 0x01)
        {
            FunctionCode = 0x05;
            UnitIdentifier = Unit;
            Header.Length = FixedLength;
        }

        public ModbusRequest05(int offset, bool value, byte Unit = 0x01)
            : this(Unit)
        {
            ReferenceNumber = (short)offset;
            Value = value ? HighValue : LowValue;
        }

        [MarshalAs(UnmanagedType.U2)]
        public short Value;

        public override byte[] ToNetworkBuffer()
        {
            var copy = (ModbusRequest05)MemberwiseClone();
            copy.Header = Header.Clone();
            copy.ApplyNetworkOrderForBase();

            copy.Value = IPAddress.HostToNetworkOrder(copy.Value);

            return copy.ToNetworkBytes();
        }
    }
}
