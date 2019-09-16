using System;
using System.Net;

namespace ModbusTcp.Protocol.Reply
{
    class ModbusWriteSingleResponseBase : ModbusResponseBase
    {
        public short ReferenceNumber;

        public short Value;

        public override void FromNetworkBuffer(byte[] buffer)
        {
            var idx = StandardResponseFromNetworkBuffer(buffer);
            
            ReferenceNumber = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, idx));
            Value = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, idx + 2));
        }
    }
}
