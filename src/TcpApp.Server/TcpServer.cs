using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TcpApp.Shared;
using static System.Console;

namespace TcpApp.Server
{
    public class TcpServer
    {
        public async Task Start(int port = SocketConfig.Port)
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, SocketConfig.Port);
            var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endpoint);
            socket.Listen(128);

            await Run(socket);
        }

        private async Task Run(Socket socket)
        {
            WriteLine("Socket server running!");

            while (true)
            {
                var clientSocket = await Task.Factory.FromAsync(
                    new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept),
                    new Func<IAsyncResult, Socket>(socket.EndAccept),
                    null
                ).ConfigureAwait(false);

                WriteLine("Connection received");

                var networkStream = new NetworkStream(clientSocket);
                var (dataType, body) = await Protocol.ReceiveAsync(networkStream);

                var protocolHandler = new ProtocolHandler(dataType);
                protocolHandler.Handle(body);

                await SendJsonData(networkStream);
            }
        }

        private async Task SendJsonData(NetworkStream networkStream)
        {
            var jsonString = JsonSerializer.Serialize(new[]{
                    new {
                        Name = "Luiz",
                        Age = 24,
                    },
                    new {
                        Name = "Matundo",
                        Age = 39
                    }
                });

            var data = Encoding.UTF8.GetBytes(jsonString);
            await Protocol.SendAsync(networkStream, DataType.Json, data);
        }
    }
}