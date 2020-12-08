namespace RobSharper.Ros.BagReader.Records
{
    public class ChunkInfoItem
    {
        public int ConnectionId { get; }
        public int Count { get; }
        
        public ChunkInfoItem(int connectionId, int count)
        {
            ConnectionId = connectionId;
            Count = count;
        }
    }
}