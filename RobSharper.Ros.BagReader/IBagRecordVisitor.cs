using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public interface IBagRecordVisitor
    {
        void Visit(BagHeader record);
        void Visit(Chunk record);
        void Visit(Connection record);
        void Visit(MessageData record);
        void Visit(IndexData record);
        void Visit(ChunkInfo record);
        void Reset();
    }
}