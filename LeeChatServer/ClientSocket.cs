using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WordCrushServer;

namespace LeeChatServer
{
    public class ClientSocket
    {
        public Socket _Socket { get; private set; }

        private Player _Player;

        private Thread _SendThread;

        private Thread _ReceiveThread;

        private ConcurrentQueue<byte[]> _SendQueue;

        private byte[] _ReceiveBuffer = new byte[1024];

        private byte[] _SendBuffer = new byte[1024];

        public string _RemoteEndPoint { get; private set; }

        public ClientSocket(Socket socket, Player player)
        {
            _Socket = socket;
            _Player = player;

            _Player.clientSocket = this;

            _RemoteEndPoint = _Socket.RemoteEndPoint.ToString();

            _SendQueue = new ConcurrentQueue<byte[]>();

            //发送消息线程
            _SendThread = new Thread(_Send);
            _SendThread.Start();
            //接收消息线程
            _ReceiveThread = new Thread(_Receive);
            _ReceiveThread.Start();
        }

        private void _Send()
        {
            while (_Player.State == State.Connected)
            {
                if(_SendQueue.Count > 0)
                {
                    try
                    {
                        _SendQueue.TryDequeue(out _SendBuffer);
                        _Socket.Send(_SendBuffer);
                        Console.WriteLine("向玩家{0}发送消息成功  长度为{1}   {2}", _RemoteEndPoint, _SendBuffer.Length, DateTime.Now);
                    }
                    catch
                    {
                        Console.WriteLine("向玩家{0}发送消息失败   {1}", _RemoteEndPoint, DateTime.Now);
                    }
                }

                Thread.Sleep(100);
            }
        }

        public void Send(MessageID id, byte[] data = null)
        {
            _SendQueue.Enqueue(NetworkUtils.Pack(id, data));
        }

        bool isCompleted;
        private void _Receive()
        {
            while(_Player.State == State.Connected)
            {
                isCompleted = false;
                _Socket.BeginReceive(_ReceiveBuffer, 0, _ReceiveBuffer.Length, SocketFlags.None, _ReceiveCallBack, _Socket);
                Console.WriteLine("正在接收...");
                Thread.Sleep(100);
                while (!isCompleted) { }
            }
        }

        private void _ReceiveCallBack(IAsyncResult async)
        {
            try //防止客户端断开连接
            {
                int receive = _Socket.EndReceive(async);//接收消息长度

                if (receive > 0)//接收到了数据
                {
                    //receive = _Socket.Receive(_Buffer);
                    Console.WriteLine("接收到了数据  len:{0}    时间：{1}", receive, DateTime.Now);
                    using (MemoryStream stream = new MemoryStream(_ReceiveBuffer))
                    {
                        BinaryReader br = new BinaryReader(stream);
                        try
                        {
                            MessageID id = (MessageID)br.ReadInt32();
                            int length = br.ReadInt32();
                            if (Server._callBacks.ContainsKey(id))
                            {
                                CallBack callBack = new CallBack(_Player, br.ReadBytes(length), Server._callBacks[id]);
                                Server._callBackQueue.Enqueue(callBack);
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"{_Socket.RemoteEndPoint}已掉线    {DateTime.Now}");
                            Close();
                            return;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("没有接收到数据， 客户端{0}断开连接", _RemoteEndPoint);
                    Close();
                }
            }
            catch
            {
                Console.WriteLine("出现异常， 客户端{0}断开连接", _RemoteEndPoint);
                Close();
            }
            isCompleted = true;
        }

        public void Close()
        {
            //_SendThread.Abort();
            //_ReceiveThread.Abort();

            _RemoteEndPoint = "";

            _SendQueue.Clear();

            _Socket.Close();
            _Player.Offline();
        }
    }
}
