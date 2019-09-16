using System;
using System.Net;

namespace ModbusTcp.Protocol.Reply
{
    class ModbusWriteRangeResponseBase : ModbusResponseBase
    {
        public short ReferenceNumber;

        public short WordCount;

        public override void FromNetworkBuffer(byte[] buffer)
        {
            var idx = StandardResponseFromNetworkBuffer(buffer);

            ReferenceNumber = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, idx));
            WordCount = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, idx + 2));
        }
    }
}
