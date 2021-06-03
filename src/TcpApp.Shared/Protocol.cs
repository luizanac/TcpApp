using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TcpApp.Shared
{
    public static class Protocol
    {

        public static async Task<(TransportCode transportCode, int bodyLength, byte[] body)> ReceiveAsync(NetworkStream networkStream)
        {
            var transportCodeByte = (await ReadAsync(networkStream, sizeof(byte)))[0];
            
            var transportCode = (TransportCode)transportCodeByte;
            var bodyLengthBytes = await ReadAsync(networkStream, sizeof(int));

            var bodyLength = BitConverter.ToInt32(bodyLengthBytes, 0);
            var bodyBytes = await ReadAsync(networkStream, bodyLength);

            return (transportCode, bodyLength, bodyBytes);
        }

        public static async Task<byte[]> ReadAsync(NetworkStream networkStream, int bytesToRead)
        {
            var buffer = new byte[bytesToRead];
            var bytesReaded = 0;
            while (bytesReaded < bytesToRead)
            {
                var bytesReceived = await networkStream.ReadAsync(buffer, bytesReaded, (bytesToRead - bytesReaded)).ConfigureAwait(false);
                if (bytesReceived == 0)
                    throw new Exception("Socket closed");
                bytesReaded += bytesReceived;
            }

            return buffer;
        }
    }
}