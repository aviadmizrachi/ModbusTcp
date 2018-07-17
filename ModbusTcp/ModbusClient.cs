using System;
using System.Net.Sockets;
using ModbusTcp.Protocol.Request;
using System.Threading.Tasks;
using ModbusTcp.Protocol;
using System.Linq;
using ModbusTcp.Protocol.Reply;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace ModbusTcp
{
    public class ModbusClient
    {
        private CancellationToken cancellationToken;
        private const int defaultSocketTimeout = 1800 * 1000; // 30 minutes
        private readonly int port;
        private TcpClient tcpClient;
        private NetworkStream transportStream;
        private readonly string ipAddress;

        public ModbusClient(string ipAddress, int port, int socketTimeoutInMs = defaultSocketTimeout)
        {
            this.ipAddress = ipAddress;
            this.port = port;

            // We'll pass this CancellationToken around to time-bound our network calls
            Console.WriteLine($"SocketTimeout is {socketTimeoutInMs} ms.");
            var cancellationTokenSource = new CancellationTokenSource(socketTimeoutInMs);
            cancellationTokenSource.Token.Register(() => transportStream.Close());
            cancellationToken = cancellationTokenSource.Token;
            cancellationToken.ThrowIfCancellationRequested();
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

        /// <summary>
        /// Reads words holding registers
        /// </summary>
        /// <param name="offset">The register offset</param>
        /// <param name="count">Number of words to read</param>
        /// <returns>The words read</returns>
        public async Task<short[]> ReadRegistersAsync(int offset, int count)
        {
            if (tcpClient == null)
                throw new Exception("Object not intialized");

            var request = new ModbusRequest03(offset, count);
            var buffer = request.ToNetworkBuffer();
            await transportStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);

            var response = await ReadResponseAsync<ModbusReply03>();
            return ReadAsShort(response.Data);
        }

        /// <summary>
        /// Reads floats from holding registers
        /// </summary>
        /// <param name="offset">The register offset</param>
        /// <param name="count">Number of floats to read</param>
        /// <returns>The floats read</returns>
        public async Task<float[]> ReadRegistersFloatsAsync(int offset, int count)
        {
            if (tcpClient == null)
                throw new Exception("Object not intialized");

            var request = new ModbusRequest03(offset, count * 2 /* Float is 2 word */);

            var buffer = request.ToNetworkBuffer();
            await transportStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);

            var response = await ReadResponseAsync<ModbusReply03>();
            return ReadAsFloat(response.Data);
        }

        /// <summary>
        /// Writes floats to holding registers
        /// </summary>
        /// <param name="offset">The first register offset</param>
        /// <param name="values">The values to write</param>
        /// <returns>Awaitable task</returns>
        public async Task WriteRegistersAsync(int offset, float[] values)
        {
            if (tcpClient == null)
                throw new Exception("Object not intialized");

            var request = new ModbusRequest16(offset, values);
            var buffer = request.ToNetworkBuffer();
            await transportStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);

            var response = await ReadResponseAsync<ModbusReply16>();
        }

        /// <summary>
        /// Writes words to holding registers
        /// </summary>
        /// <param name="offset">The first register offset</param>
        /// <param name="values">The values to write</param>
        /// <returns>Awaitable task</returns>
        public async Task WriteRegistersAsync(int offset, short[] values)
        {
            if (tcpClient == null)
                throw new Exception("Object not intialized");

            var request = new ModbusRequest16();
            request.WordCount = (short)(values.Length * 2);
            request.RegisterValues = values.ToNetworkBytes();

            var buffer = request.ToNetworkBuffer();
            await transportStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);

            var response = await ReadResponseAsync<ModbusReply16>();
        }

        /// <summary>
        /// Terminates the session
        /// </summary>
        public void Terminate()
        {
            tcpClient.Close();
            tcpClient = null;
        }

        private short[] ReadAsShort(byte[] data)
        {
            var idx = 0;
            var output = new List<short>();

            while (idx < data.Length)
            {
                var value = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, idx));
                idx += 2;

                output.Add(value);
            }

            return output.ToArray();
        }

        private float[] ReadAsFloat(byte[] data)
        {
            var idx = 0;
            var output = new List<float>();

            while (idx < data.Length)
            {
                var value = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, idx));
                var f = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
                idx += 4;

                output.Add(f);
            }

            return output.ToArray();
        }

        private async Task<T> ReadResponseAsync<T>() where T : ModbusReponseBase
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
                var readBytes = await transportStream.ReadAsync(buffer, idx, remainder, cancellationToken);
                remainder -= readBytes;
                idx += readBytes;

                if (readBytes == 0)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new TimeoutException("SocketTimeout reached, aborting network call. " +
                            "You can adjust the timeout through the socketTimeoutInSeconds parameter of ModbusClient().");
                    else
                        throw new SocketException((int)SocketError.ConnectionReset);
                }
            }

            return buffer;
        }
    }
}
