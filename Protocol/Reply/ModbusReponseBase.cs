using System;

namespace ModbusTcp.Protocol.Reply
{
    abstract class ModbusReponseBase : ModbusBase
    {
        public abstract void FromNetworkBuffer(byte[] buffer);

        public override sealed byte[] ToNetworkBuffer()
        {
            throw new NotImplementedException();
        }
    }
}
