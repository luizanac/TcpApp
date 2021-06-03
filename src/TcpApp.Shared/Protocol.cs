using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpApp.Shared
{
    public static class Protocol
    {

        public static async Task<(DataType dataType, byte[] body)> ReceiveAsync(NetworkStream networkStream)
        {
            var dataType = (DataType)(await ReadAsync(networkStream, sizeof(byte)))[0];

            var headerBytes = await ReadAsync(networkStream, sizeof(int));
            var header = BitConverter.ToInt32(headerBytes, 0);

            var body = await ReadAsync(networkStream, header);

            return (dataType, body);
        }

        public static async Task SendAsync(NetworkStream networkStream, DataType dataType, byte[] data, CancellationToken cancellationToken = default)
        {
            var (dataTypeBytes, header, body) = Encode(dataType, data);
            await networkStream.WriteAsync(dataTypeBytes, 0, dataTypeBytes.Length, cancellationToken);
            await networkStream.WriteAsync(header, 0, header.Length, cancellationToken);
            await networkStream.WriteAsync(body, 0, body.Length, cancellationToken);
        }

        static (byte[] dataType, byte[] header, byte[] body) Encode(DataType dataType, byte[] data)
        {
            var dataTypeBytes = new byte[] { (byte)dataType };
            var body = data;
            var header = BitConverter.GetBytes(body.Length);
            return (dataTypeBytes, header, body);
        }

        public static async Task<byte[]> ReadAsync(NetworkStream networkStream, int bytesToRead)
        {
            var buffer = new byte[bytesToRead];
            var bytesReaded = 0;
            while (bytesReaded < bytesToRead)
            {
                var bytesReceived = await networkStream.ReadAsync(buffer, bytesReaded, (bytesToRead - bytesReaded)).ConfigureAwait(false);
                if (bytesReceived == 0)
                    throw new Exception("Socket closed!");
                bytesReaded += bytesReceived;
            }

            return buffer;
        }
    }
}