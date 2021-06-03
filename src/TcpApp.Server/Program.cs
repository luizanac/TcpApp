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
    while (true)
    {
        WriteLine("Waiting for a connection...");

        var client = server.AcceptTcpClient();
        WriteLine("Client connected!");

        var networkStream = client.GetStream();
        var (transportCode, bodyLength, bodyBytes) = Protocol.ReceiveAsync(networkStream).Result;

        switch (transportCode)
        {
            case TransportCode.Json:
                WriteLine("Processing JSON...");
                break;
            case TransportCode.File:
                WriteLine("Processing FILE");
                break;
            default:
                WriteLine("Unrecognized TranportCode formart!");
                break;
        }

        var message = Encoding.UTF8.GetBytes($"Your {transportCode.ToString()} is being processed");
        networkStream.Write(message, 0, message.Length);

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
