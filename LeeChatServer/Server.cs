using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LeeChatServer
{
    public delegate void ServerCallBack(Player player, byte[] data);

    public class CallBack 
    {
        public Player Player;

        public byte[] Data;

        public ServerCallBack ServerCallBack;

        public CallBack(Player player, byte[] data, ServerCallBack serverCallBack)
        {
            Player = player;
            Data = data;
            ServerCallBack = serverCallBack;
        }

        public void Execute()
        {
            ServerCallBack(Player, Data);
        }
    }

    public static class Server
    {
        public static ConcurrentQueue<CallBack> _callBackQueue;   //回调方法队列

        public static Dictionary<MessageID, ServerCallBack> _callBacks
            = new Dictionary<MessageID, ServerCallBack>();

        private static Socket ServerSocket; //服务器socket

        public static List<Player> Players;
        public static Dictionary<int, Room> Rooms;

        #region 线程相关
        public static void _CallBack()
        {
            while (true)
            {
                if(_callBackQueue.Count > 0)
                    if(_callBackQueue.TryDequeue(out CallBack callBack))
                        callBack.Execute();  //执行回调
                Thread.Sleep(10);
            }
        }

        private static void _Await()
        {
            Socket client = null;
            while (true)
            {
                try
                {
                    //同步等待
                    client = ServerSocket.Accept();

                    //获取客户端的唯一键
                    string endPoint = client.RemoteEndPoint.ToString();

                    //新增玩家
                    Player player = new Player();
                    ClientSocket clientSocket = new ClientSocket(client, player);
                    Players.Add(player);

                    Console.WriteLine($"{endPoint}连接成功");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        //启动服务器
        public static void Start(string ip, int port = 8848)
        {
            _callBackQueue = new ConcurrentQueue<CallBack>();
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Players = new List<Player>();

            ServerSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

            ServerSocket.Listen(3000);

            //开启等待玩家线程
            Thread await = new Thread(_Await) { IsBackground = true };
            await.Start();

            //开启回调方法线程
            Thread callback = new Thread(_CallBack) { IsBackground = true };
            callback.Start();
        }

        //注册消息回调事件
        public static void Register(MessageID id, ServerCallBack method)
        {
            if (!_callBacks.ContainsKey(id))
                _callBacks.Add(id, method);
            else
                Console.WriteLine("注册了相同的回调事件！");
        }
    }
}
