using System;
using System.Net;

namespace ModbusTcp.Protocol.Reply
{
    class ModbusReply16 : ModbusReponseBase
    {
        public byte UnitIdentifier;

        public byte FunctionCode;

        public short ReferenceNumber;

        public short WordCount;

        public ModbusReply16()
        {
            
        }

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
                ReferenceNumber = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, idx));
                idx += 2;

                WordCount = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, idx));
                idx += 2;
            }
        }
    }
}
