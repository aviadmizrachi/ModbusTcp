using System.Net;
using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public abstract class ModbusBase
    {
        public ModbusBase()
        {
            Header = new ModbusHeader();
        }

        public ModbusHeader Header { get; set; }

        public abstract byte[] ToNetworkBuffer();
    }
}
