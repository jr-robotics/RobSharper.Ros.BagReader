using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public interface IBagRecordVisitor
    {
        void Visit(BagHeader record);
        void Visit(Chunk record);
        void Visit(Connection record);
    }
}