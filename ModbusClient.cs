using System;
using System.Net.Sockets;
using ModbusTcp.Protocol.Request;
using System.Threading.Tasks;
using ModbusTcp.Protocol;
using System.Linq;
using ModbusTcp.Protocol.Reply;

namespace ModbusTcp
{
    public class ModbusClient
    {
        private readonly int port;
        private TcpClient tcpClient;
        private NetworkStream transportStream;
        private readonly string ipAddress;

        public ModbusClient(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public void Init()
        {
            try
            {
                tcpClient = new TcpClient(ipAddress, port);
                transportStream = tcpClient.GetStream();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to open TCP client - {e}");
                throw e;
            }
        }

        public async Task<short[]> ReadRegistersAsync(int offset, int count)
        {
            if (tcpClient == null)
                throw new Exception("Object not intialized");

            var request = new ModbusRequest03();
            request.ReferenceNumber = (short)offset;
            request.WordCount = (short)count;

            var buffer = request.ToNetworkBuffer();
            await transportStream.WriteAsync(buffer, 0, buffer.Length);

            var response = await ReadResponse<ModbusReply03>();
            return response.Data;
        }

        private async Task<T> ReadResponse<T>() where T : ModbusReponseBase
        {
            var headerBytes = await ReadFromBuffer(ModbusHeader.FixedLength);
            var header = ModbusHeader.FromNetworkBuffer(headerBytes);

            var dataBytes = await ReadFromBuffer(header.Length);

            var fullBuffer = headerBytes.Concat(dataBytes).ToArray();
            var response = Activator.CreateInstance<T>();
            response.FromNetworkBuffer(fullBuffer);

            return response;
        }

        private async Task<byte[]> ReadFromBuffer(int totalSize)
        {
            var buffer = new byte[totalSize];

            var idx = 0;
            var remainder = totalSize;

            while (remainder > 0)
            {
                var readBytes = await transportStream.ReadAsync(buffer, idx, remainder);
                remainder -= readBytes;
                idx += readBytes;

                if (readBytes == 0)
                {
                    throw new SocketException((int)SocketError.ConnectionReset);
                }
            }

            return buffer;
        }

        //public float[] ReadRegistersAsFloat(int offset, int wordCount)
        //{
        //    if (tcpClient == null)
        //        throw new Exception("Object not intialized");


        //}
    }
}
