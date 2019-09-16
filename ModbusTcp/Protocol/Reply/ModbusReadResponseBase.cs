using System;

namespace ModbusTcp.Protocol.Reply
{
    class ModbusReadResponseBase : ModbusResponseBase
    {
        public byte Length;

        public byte[] Data;

        public override void FromNetworkBuffer(byte[] buffer)
        {
            var idx = StandardResponseFromNetworkBuffer(buffer);

            Length = buffer[idx++];

            Data = new byte[Length];
            Buffer.BlockCopy(buffer, idx, Data, 0, Length);
        }
    }
}
