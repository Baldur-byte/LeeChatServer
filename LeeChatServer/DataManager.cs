using Newtonsoft.Json;
using Protobuf;
using System;

namespace LeeChatServer
{
    public static class DataManager
    {
        private const string UserInfoPath = "userinfo.json";

        private const string FriendListPath = "friendlist.json";

        private static string users = "";

        private static string friends = "";

        private static List<PlayerInfo> userList = new List<PlayerInfo>();

        private static Dictionary<string, List<string>> friendList = new Dictionary<string, List<string>>();

        private static bool isInitlized 
        { 
            get 
            { 
                return userList.Count == 0 &&
                    friendList.Count == 0 && 
                    users == "" &&
                    friends == ""; 
            } 
        }

        private static void updateCache()
        {
            users = JsonTools.Read(UserInfoPath);
            friends = JsonTools.Read(FriendListPath);

            if (string.IsNullOrEmpty(users) || string.IsNullOrEmpty(friends)) return;

            userList = JsonConvert.DeserializeObject<List<PlayerInfo>>(users);
            friendList = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(friends);
        }

        public static bool isIdExist(string uuid)
        {
            if (!isInitlized) updateCache();
            foreach (var info in userList)
                if (info.Uuid == uuid) return true;
            return false;
        }

        public static bool isNameExist(string name)
        {
            if (!isInitlized) updateCache();
            foreach (var info in userList)
                if (info.Name == name) return true;
            return false;
        }

        public static PlayerInfo GetInfoById(string uuid)
        {
            if (!isInitlized) updateCache();
            foreach (var info in userList)
                if (info.Uuid == uuid) return info;
            return new PlayerInfo();
        }

        public static PlayerInfo GetInfoByName(string name)
        {
            if (!isInitlized) updateCache();
            foreach (var info in userList)
                if (info.Name == name) return info;
            return new PlayerInfo();
        }

        public static List<string> GetFriendsById(string uuid)
        {
            if (!isInitlized) updateCache();
            List<string> list = new List<string>();
            friendList.TryGetValue(uuid, out list);
            return list;
        }

        public static bool CheckPassword(string uuid, string password)
        {
            if (!isInitlized) updateCache();
            if (userList != null && isIdExist(uuid) && GetInfoById(uuid).Password == password)
                return true;
            return false;
        }

        //文件内容增减
        public static bool AddUser(RegisterCS register)
        {
            if (!isInitlized) updateCache();
            if (isIdExist(register.Uuid)) return false;
            PlayerInfo info = new PlayerInfo();
            info.Uuid = register.Uuid;
            info.Password = register.Password;
            info.Name = register.Name;
            info.IconUrl = "www.baidu.com";
            userList.Add(info);
            users = JsonConvert.SerializeObject(userList);
            JsonTools.Write(UserInfoPath, users);
            return true;
        }

        public static void AddFriend(string uuid, string friendId)
        {
            if (!isInitlized) updateCache();
            if (isIdExist(uuid))
            {
                if (friendList.ContainsKey(uuid))
                {
                    List<string> list = friendList[uuid];
                    if (list.Contains(friendId))
                    {
                        Console.WriteLine("存在该好友");
                    }
                    else
                    {
                        list.Add(friendId);
                        friends = JsonConvert.SerializeObject(friendList);
                        JsonTools.Write(FriendListPath, friends);
                    }
                }
            }
            else
            {
                Console.WriteLine("该用户不存在！");
            }
        }

        public static void RemoveUser(string uuid)
        {
            if (!isInitlized) updateCache();
            foreach (var info in userList)
            {
                if (info.Uuid == uuid)
                {
                    userList.Remove(info);
                    Console.WriteLine("用户列表移除成功，开始移除好友列表");
                }
            }

            users = JsonConvert.SerializeObject(userList);
            JsonTools.Write(UserInfoPath, users);

            if (friendList.ContainsKey(uuid)) friendList.Remove(uuid);
            List<string> list;
            foreach(var item in friendList)
            {
                list = item.Value;
                if (list.Contains(uuid))
                {
                    list.Remove(uuid);
                }
            }

            friends = JsonConvert.SerializeObject(friendList);
            JsonTools.Write(FriendListPath, friends);
        }

        public static void RemoveFriend(string uuid, string friendId)
        {
            if (!isInitlized) updateCache();
            if (isIdExist(uuid))
            {
                if (friendList.ContainsKey(uuid))
                {
                    List<string> list = friendList[uuid];
                    if (list.Contains(friendId))
                    {
                        list.Remove(friendId);
                        friends = JsonConvert.SerializeObject(friendList);
                        JsonTools.Write(FriendListPath, friends);
                    }
                    else
                    {
                        Console.WriteLine("不存在该好友");
                    }
                }
            }
            else
            {
                Console.WriteLine("该用户不存在！");
                if (friendList.ContainsKey(uuid))
                {
                    friendList.Remove(uuid);
                }
            }
        }
    }
}
