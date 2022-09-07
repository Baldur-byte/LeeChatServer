namespace LeeChatServer
{
    public class Player
    {
        public string name { get; private set; }

        public string id { get; private set; }

        public ClientSocket clientSocket;

        public State State { get; private set; }

        public Player() { State = State.Connected; }

        public Player(string id, string name = "Player Unknown") : this()
        {
            this.name = name;
            this.id = id;
        }

        public void Offline()
        {
            State = State.Disconnected;
            Server.Players.Remove(this);
        }
    }

    public enum State
    {
        Connected,
        Disconnected,
    }
}
