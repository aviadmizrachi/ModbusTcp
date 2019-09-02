using System;

namespace ModbusTcp.Protocol.Reply
{
    class ModbusReply02 : ModbusReponseBase
    {
        public byte UnitIdentifier;

        public byte FunctionCode;

        public byte Length;

        public byte[] Data;

        public override void FromNetworkBuffer(byte[] buffer)
        {
            var idx = 0;

            Header = ModbusHeader.FromNetworkBuffer(buffer);
            idx += ModbusHeader.FixedLength;

            UnitIdentifier = buffer[idx++];
            FunctionCode = buffer[idx++];

            if (FunctionCode >= 0x80)
            {
                var exceptionCode = buffer[idx];
                throw new ModbusReplyException(exceptionCode);
            }
            else
            {
                Length = buffer[idx++];

                Data = new byte[Length];
                Buffer.BlockCopy(buffer, idx, Data, 0, Length);
            }
        }
    }
}
