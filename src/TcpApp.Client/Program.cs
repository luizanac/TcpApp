using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using TcpApp.Shared;
using static System.Console;

static void Connect(string data)
{
    try
    {
        var client = new TcpClient(TcpConfig.ServerAddress, TcpConfig.Port);

        var stream = client.GetStream();

        var code = (uint)TransportCode.File;
        WriteLine(code);
        var codeBytes = BitConverter.GetBytes(code);
        stream.Write(codeBytes, 0, codeBytes.Length);

        Thread.Sleep(5000);

        var dataBytes = Encoding.UTF8.GetBytes(data);
        stream.Write(dataBytes, 0, dataBytes.Length);

        var buffer = new Byte[1024*1024];
        var bytesReaded = stream.Read(buffer, 0, buffer.Length);
        var responseData = Encoding.UTF8.GetString(buffer, 0, bytesReaded);
        WriteLine("Received data: {0} ", responseData);

        stream.Close();
        client.Close();
    }
    catch (ArgumentNullException e)
    {
        WriteLine("ArgumentNullException: {0}", e);
    }
    catch (SocketException e)
    {
        WriteLine("SocketException: {0}", e);
    }
}

WriteLine("Initializing connection...");

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
Connect(jsonString);