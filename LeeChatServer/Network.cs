using Google.Protobuf;
using Protobuf;
using Json.Net;

namespace LeeChatServer
{
    public class Network
    {
        public Network(string ip)
        {
            //注册事件
            Server.Register(MessageID.LoginCS, LoginCallBack);
            Server.Register(MessageID.RegisterCS, RegisterCallBack);
            Server.Register(MessageID.Ping, PingCallBack);
            Server.Register(MessageID.ConnectionCS, ConnectionCallBack);
            Server.Register(MessageID.ErrorCode, ErrorCodeCallBack);
            Server.Register(MessageID.UpdateInfoCS, UpdateInfoCallBack);
            Server.Register(MessageID.GetPlayerInfosCS, GetPlayerInfosCallBack);
            Server.Register(MessageID.SendToOne, SendToOneCallBack);
            Server.Register(MessageID.SendToGroup, SendToGroupCallBack);

            //开启服务器
            Server.Start(ip);
        }

        private void LoginCallBack(Player player, byte[] data)
        {
            Console.WriteLine($"接收到登录请求---(From: Player{player.id}  {NetworkUtils.GetCurrentTime()})");
            LoginCS loginCS = new LoginCS();
            loginCS = NetworkUtils.GetProto(loginCS, data) as LoginCS;

            Console.WriteLine("验证用户ID和密码...");
            player.clientSocket.Send(MessageID.LoginSC, LoginMethod(loginCS).ToByteArray());
        }

        private void RegisterCallBack(Player player, byte[] data)
        {
            Console.WriteLine($"接收到注册请求---(From: Player{player.id}  {NetworkUtils.GetCurrentTime()})");
            RegisterCS registerCS = new RegisterCS();
            registerCS = NetworkUtils.GetProto(registerCS, data) as RegisterCS;

            Console.WriteLine("验证用户ID和密码...");
            player.clientSocket.Send(MessageID.RegisterSC, RegisterMethod(registerCS).ToByteArray());
        }

        private void PingCallBack(Player player, byte[] data)
        {
            Console.WriteLine($"接收到心跳包---(From: Player{player.id}  {NetworkUtils.GetCurrentTime()})");
            player.clientSocket.Send(MessageID.Pong);

        }

        private void ConnectionCallBack(Player player, byte[] data)
        {
            Console.WriteLine($"接收到连接请求---(From: Player{player.id}  {NetworkUtils.GetCurrentTime()})");

        }

        private void ErrorCodeCallBack(Player player, byte[] data)
        {
            Console.WriteLine($"接收到错误码---(From: Player{player.id}  {NetworkUtils.GetCurrentTime()})");

        }

        private void UpdateInfoCallBack(Player player, byte[] data)
        {
            Console.WriteLine($"接收到更新玩家信息请求---(From: Player{player.id}  {NetworkUtils.GetCurrentTime()})");

        }

        private void GetPlayerInfosCallBack(Player player, byte[] data)
        {
            Console.WriteLine($"接收到获取玩家信息请求---(From: Player{player.id}  {NetworkUtils.GetCurrentTime()})");

        }

        private void SendToOneCallBack(Player player, byte[] data)
        {
            Console.WriteLine($"接收到发送消息到单个玩家请求---(From: Player{player.id}  {NetworkUtils.GetCurrentTime()})");

        }

        private void SendToGroupCallBack(Player player, byte[] data)
        {
            Console.WriteLine($"接收到发送消息到一组玩家请求---(From: Player{player.id}  {NetworkUtils.GetCurrentTime()})");

        }

        #region 辅助方法
        private LoginSC LoginMethod(LoginCS loginCS)
        {
            LoginSC loginSC = new LoginSC();
            if (!JsonTools.isIdExist(loginCS.Uuid))
            {
                loginSC.Result = false;
                Console.WriteLine($"用户{loginCS.Uuid}不存在！");
                return loginSC;
            }
            if (!JsonTools.CheckPassword(loginCS.Uuid, loginCS.Password))
            {
                loginSC.Result = false;
                Console.WriteLine($"用户{loginCS.Uuid}密码错误！");
                return loginSC;
            }
            loginSC.Info = JsonTools.GetInfoById(loginCS.Uuid);
            loginSC.Result = true;
            return loginSC;
        }

        private RegisterSC RegisterMethod(RegisterCS registerCS)
        {
            RegisterSC registerSC = new RegisterSC();
            if (JsonTools.AddUser(registerCS))
            {
                registerSC.Info = JsonTools.GetInfoById(registerCS.Uuid);
                registerSC.Result = true;
                return registerSC;
            }
            registerSC.Result = false;
            return registerSC;
        }
        #endregion
    }
}
