namespace WordCrushServer
{
    public class MessagePacker
    {
        private List<byte> bytes = new List<byte>();

        public byte[] Packeage
        {
            get { return bytes.ToArray(); }
        }

        public MessagePacker Add(byte value)
        {
            bytes.Add(value);
            return this;
        }

        public MessagePacker Add(byte[] value)
        {
            bytes.AddRange(value);
            return this;
        }

        public MessagePacker Add(int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            bytes.AddRange(data);
            return this;
        }

        public MessagePacker Add(long value)
        {
            byte[] data = BitConverter.GetBytes(value);
            bytes.AddRange(data);
            return this;
        }
    }
}