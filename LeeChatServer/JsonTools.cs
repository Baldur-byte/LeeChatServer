using Newtonsoft.Json;
using Protobuf;
using System;
using System.IO;
using System.Xml.Linq;

namespace LeeChatServer
{
    public static class JsonTools
    {
        private const string InfoPath = "userinfo.json";

        private static string userInfo;

        private static List<PlayerInfo> userInfoList;

        private static bool isOpening = false;
        private static void Read()
        {
            userInfoList = new List<PlayerInfo>();
            userInfo = "";
            while (isOpening)
            {
                //Console.WriteLine("读文件出错，文件已经打开");
                //return;
            }
            isOpening = true;
            try
            {
                if (!File.Exists(InfoPath))
                {
                    Console.WriteLine("文件不存在！");
                    File.Create(InfoPath);
                    return;
                }
                userInfoList = new List<PlayerInfo>();
                using (StreamReader sr = new StreamReader(InfoPath))
                {
                    userInfo = sr.ReadToEnd();
                    if(!string.IsNullOrEmpty(userInfo))
                    {
                        userInfoList = JsonConvert.DeserializeObject<List<PlayerInfo>>(userInfo);
                    }
                    sr.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("读文件出错：" + ex.Message);
            }
            finally
            {
                isOpening = false;
            }
        }

        private static void Write()
        {
            while (isOpening)
            {
                //Console.WriteLine("写文件出错，文件已经打开");
                //return;
            }
            isOpening = true;
            try
            {
                if (File.Exists(InfoPath)) File.Delete(InfoPath);
                File.Create(InfoPath).Dispose();
                using (StreamWriter sr = new StreamWriter(InfoPath))
                {
                    userInfo = JsonConvert.SerializeObject(userInfoList);
                    
                    sr.Write(userInfo);
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("写文件出错：" + ex.Message);
            }
            finally 
            { 
                isOpening = false; 
            }
        }

        public static bool isIdExist(string uuid)
        {
            Read();
            if(userInfoList != null)
                foreach( var info in userInfoList)
                    if (info.Uuid == uuid) return true;
            return false;
        }

        public static bool isNameExist(string name)
        {
            Read();
            if (userInfoList != null)
                foreach (var info in userInfoList)
                    if (info.Name == name) return true;
            return false;
        }

        public static PlayerInfo GetInfoById(string uuid)
        {
            Read();
            if (userInfoList != null)
                foreach (var info in userInfoList)
                    if (info.Uuid == uuid) return info;
            return null;
        }

        public static PlayerInfo GetInfoByName(string name)
        {
            Read();
            if (userInfoList != null)
                foreach (var info in userInfoList)
                    if (info.Name == name) return info;
            return null;
        }

        public static bool CheckPassword(string uuid, string password)
        {
            Read();
            if (userInfoList != null && isIdExist(uuid) && GetInfoById(uuid).Password == password)
                return true;
            return false;
        }

        public static bool AddUser(RegisterCS register)
        {
            Read();
            if (isIdExist(register.Uuid)) return false;
            PlayerInfo info = new PlayerInfo();
            info.Uuid = register.Uuid;
            info.Password = register.Password;
            info.Name = register.Name;
            info.IconUrl = "www.baidu.com";
            userInfoList.Add(info);
            Write();
            return true;
        }
    }
}
