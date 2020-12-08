using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public class NullVisitor : IBagRecordVisitor
    {
        public static readonly NullVisitor Instance = new NullVisitor();
        
        public void Visit(BagHeader record)
        {
        }

        public void Visit(Chunk record)
        {
        }

        public void Visit(Connection record)
        {
        }

        public void Visit(MessageData record)
        {
            
        }

        public void Visit(IndexData record)
        {
            
        }

        public void Visit(ChunkInfo record)
        {
            
        }
    }
}