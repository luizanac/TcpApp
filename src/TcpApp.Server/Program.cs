using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpApp.Shared;
using static System.Console;

TcpListener server = null;
try
{
    var ipAddress = IPAddress.Parse(TcpConfig.ServerAddress);
    server = new TcpListener(ipAddress, TcpConfig.Port);

    server.Start();

    var buffer = new byte[1024 * 1024];
    while (true)
    {
        WriteLine("Waiting for a connection...");

        var client = server.AcceptTcpClient();
        WriteLine("Client connected!");

        var stream = client.GetStream();
        int bytesReaded;

        TransportCode? connectionCode = default;
        while ((bytesReaded = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            if (bytesReaded == sizeof(uint))
            {
                var code = BitConverter.ToUInt32(buffer, 0);
                connectionCode = (TransportCode)code;
                WriteLine($"Code is: {code}");
            }

            if (connectionCode != null && bytesReaded > sizeof(uint))
            {
                var data = Encoding.UTF8.GetString(buffer, 0, bytesReaded);
                WriteLine($"Received data: {data}");

                switch (connectionCode)
                {
                    case TransportCode.Json:
                        WriteLine("Processing Json...");
                        break;
                    case TransportCode.File:
                        WriteLine("Processing File...");
                        break;
                    default:
                        WriteLine("unrecognized code");
                        break;
                }

                var message = Encoding.UTF8.GetBytes($"Your {connectionCode.ToString()} is being processed");
                stream.Write(message, 0, message.Length);
            }
        }

        client.Close();
    }
}
catch (SocketException e)
{
    WriteLine("SocketException: {0}", e);
}
finally
{
    server.Stop();
    WriteLine("Server has stoped!");
}
