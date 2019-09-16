using System;

namespace ModbusTcp.Protocol.Reply
{
    abstract class ModbusResponseBase : ModbusBase
    {
        private const byte ModbusErrorOffset = 0x80;

        public byte UnitIdentifier;

        public byte FunctionCode;

        public abstract void FromNetworkBuffer(byte[] buffer);

        protected int StandardResponseFromNetworkBuffer(byte[] buffer)
        {
            var idx = 0;

            Header = ModbusHeader.FromNetworkBuffer(buffer);
            idx += ModbusHeader.FixedLength;

            UnitIdentifier = buffer[idx++];
            FunctionCode = buffer[idx++];

            if (FunctionCode >= 0x80)
            {
                var exceptionCode = buffer[idx];
                var orginalFunctionCode = (byte)(FunctionCode - ModbusErrorOffset);

                throw new ModbusReplyException(orginalFunctionCode, exceptionCode);
            }

            return idx;
        }

        public override sealed byte[] ToNetworkBuffer()
        {
            throw new NotImplementedException();
        }
    }
}
