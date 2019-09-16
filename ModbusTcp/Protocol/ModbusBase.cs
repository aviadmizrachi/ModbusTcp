using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public abstract class ModbusBase
    {
        public ModbusHeader Header { get; set; } = new ModbusHeader();

        public abstract byte[] ToNetworkBuffer();
    }
}
