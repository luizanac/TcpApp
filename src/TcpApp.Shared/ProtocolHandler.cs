using System.Text;
using static System.Console;

namespace TcpApp.Shared
{
    public class ProtocolHandler
    {
        readonly DataType _dataType;

        public ProtocolHandler(DataType dataType)
        {
            _dataType = dataType;
        }

        public void Handle(byte[] body)
        {
            switch (_dataType)
            {
                case DataType.Json:
                    WriteLine($"Processing JSON: {Encoding.UTF8.GetString(body)}");
                    break;
                case DataType.Xml:
                    WriteLine($"Processing XML with {body.Length} bytes");
                    break;
                default:
                    WriteLine("Unrecognized TranportCode formart!");
                    break;
            }
        }

        
    }
}