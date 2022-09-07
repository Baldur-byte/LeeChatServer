using System.Net.Sockets;
using System.Net;
using WordCrushServer;
using Google.Protobuf;

namespace LeeChatServer 
{
    public static class NetworkUtils
    {
        public static string GetLocalIPv4()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry iPHostEntry = Dns.GetHostEntry(hostName);
            for (int i = 0; i < iPHostEntry.AddressList.Length; i++)
            {
                if (iPHostEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    return iPHostEntry.AddressList[i].ToString();
                }
            }
            return "127.0.0.1";
        }

        public static byte[] Pack(MessageID id, byte[] data)
        {
            MessagePacker packer = new MessagePacker();
            packer.Add((int)id);
            packer.Add(GetTimeStamp());
            if (data == null)
                packer.Add(0);
            else
            {
                packer.Add(data.Length);
                packer.Add(data);
            }

            return packer.Packeage;
        }

        public static long GetTimeStamp()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public static DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }

        public static IMessage GetProto(IMessage message, byte[] bytes)
        {
            return message.Descriptor.Parser.ParseFrom(bytes);
        }
    }
}
