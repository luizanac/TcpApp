using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TcpApp.Shared;
using static System.Console;

static async Task Connect(string data)
{
    var client = new TcpClient(SocketConfig.ServerAddress, SocketConfig.Port);
    var networkStream = client.GetStream();

    await Protocol.SendAsync(networkStream, DataType.Json, Encoding.UTF8.GetBytes(data));
    await Protocol.SendAsync(networkStream, DataType.Json, Encoding.UTF8.GetBytes(data));

    var (dataType, body) = await Protocol.ReceiveAsync(networkStream);

    new ProtocolHandler(dataType).Handle(body);


    networkStream.Close();
    client.Close();
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
await Connect(jsonString);