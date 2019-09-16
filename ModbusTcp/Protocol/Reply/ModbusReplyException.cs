using System;

namespace ModbusTcp.Protocol.Reply
{
    [Serializable]
    public sealed class ModbusReplyException : Exception
    {
        public byte OrginalFunctionCode { get; set; }
        public byte ExceptionCode { get; set; }

        public ModbusReplyException(byte orginalFunctionCode, byte exceptionCode) : base(GetMsg(exceptionCode))
        {
            OrginalFunctionCode = orginalFunctionCode;
            ExceptionCode = exceptionCode;
        }

        private static string GetMsg(byte exceptionCode)
        {
            switch (exceptionCode)
            {
                case 0x01:
                    return @"Illegal Function Code: The function code is unknown by the server";
                case 0x02:
                    return @"Illegal Data Address: Dependant on the request";
                case 0x03:
                    return @"Illegal Data Value: Dependant on the request";
                case 0x04:
                    return @"Server Failure: The server failed during the execution";
                case 0x05:
                    return @"Acknowledge: The server accepted the service invocation but the service requires a relatively long time to execute. The server therefore returns only an acknowledgement of the service invocation receipt.";
                case 0x06:
                    return @"Server Busy: The server was unable to accept the MB Request PDU. The client application has the responsibility of deciding if and when to re-send the request.";
                case 0x0A:
                    return @"Gateway problem: Gateway paths not available.";
                case 0x0B:
                    return @"Gateway problem: The targeted device failed to respond. The gateway generates this exception";
                default:
                    return @"Unknown exception";
            }
        }
    }
}