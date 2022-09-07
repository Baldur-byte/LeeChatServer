using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LeeChatServer
{
    public class Start
    {
        private static void Main()
        {
            string ip = NetworkUtils.GetLocalIPv4();
            new Network(ip);

            Console.WriteLine("服务器已启动");
            Console.WriteLine($"ip地址为:{ip}");

            Console.ReadKey();
        }
    }
}
