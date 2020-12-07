namespace RobSharper.Ros.BagReader.Records
{
    public static class OpCodes
    {
        public const int BagHeader = 0x03;
        public const int Chunk = 0x05;
        public const int Connection = 0x07;
        public const int MessageData = 0x02;
        public const int IndexData = 0x04;
        public const int ChunkInfo = 0x06;
    }
}