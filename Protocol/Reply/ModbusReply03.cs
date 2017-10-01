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

        public short[] Data;

        public override void FromNetworkBuffer(byte[] buffer)
        {
            var idx = 0;

            Header = ModbusHeader.FromNetworkBuffer(buffer);
            idx += ModbusHeader.FixedLength;

            UnitIdentifier = buffer[idx++];
            FunctionCode = buffer[idx++];
            Length = buffer[idx++];

            Data = new short[Length / 2];
            for (int i = 0; i < Length / 2; i++)
            {
                Data[i] = BitConverter.ToInt16(buffer, idx);
                Data[i] = IPAddress.NetworkToHostOrder(Data[i]);
                idx += 2;
            }
        }
    }
}
