using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeChatServer
{
    public class Room
    {
        public int RoomId;

        public enum RoomState
        {
            Available,
            Unavailable,
        }

        public RoomState State;

        public List<Player> PlayerList = new List<Player>();

        public const int MAX_PLAYER_AMOUNT = 2;

        public Room(int roomId)
        {
            RoomId = roomId;
            State = RoomState.Available;
        }

        public void Close()
        {
            State = RoomState.Unavailable;
        }
    }
}
