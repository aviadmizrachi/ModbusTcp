using System;
using System.Net;
using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol.Request
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ModbusRequest16 : ModbusRequestBase
    {
        public const int FixedLength = 7;

        public ModbusRequest16(byte Unit = 0x01)
        {
            FunctionCode = 0x10;
            UnitIdentifier = Unit;
        }

        public ModbusRequest16(int offset, float[] values, byte Unit = 0x01)
            : this(Unit)
        {

            ReferenceNumber = (short)offset;
            WordCount = (short)(values.Length * 2);
            RegisterValues = values.ToNetworkBytes();
            ByteCount = (byte)RegisterValues.Length;

            Header.Length = (short) (FixedLength + RegisterValues.Length);
        }

        [MarshalAs(UnmanagedType.U2)]
        public short WordCount;

        [MarshalAs(UnmanagedType.U1)]
        public byte ByteCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] RegisterValues;

        public override byte[] ToNetworkBuffer()
        {
            var copy = (ModbusRequest16)MemberwiseClone();
            copy.Header = Header.Clone();
            copy.Header = Header.Clone();
            copy.ApplyNetworkOrderForBase();

            copy.WordCount = IPAddress.HostToNetworkOrder(copy.WordCount);

            var buffer = copy.ToNetworkBytes();
            var outputBuffer = new byte[buffer.Length - 2 + RegisterValues.Length];
            Array.Copy(buffer, outputBuffer, buffer.Length - 2);
            Array.Copy(RegisterValues, 0, outputBuffer, buffer.Length - 2, RegisterValues.Length);

            return outputBuffer;
        }
    }
}
