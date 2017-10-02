using System;
using System.Net;

namespace ModbusTcp.Protocol.Reply
{
    class ModbusReply03 : ModbusReponseBase
    {
        public ModbusHeader Header;

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
            Length = buffer[idx++];

            Data = new byte[Length];
            Buffer.BlockCopy(buffer, idx, Data, 0, Length);
        }
    }
}
