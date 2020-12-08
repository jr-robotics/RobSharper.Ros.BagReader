using System;

namespace RobSharper.Ros.BagReader.Records
{
    public class IndexItem
    {
        public IndexItem(DateTime time, int chunkOffset)
        {
            Time = time;
            ChunkOffset = chunkOffset;
        }

        public DateTime Time { get; }
        public int ChunkOffset { get; }
    }
}