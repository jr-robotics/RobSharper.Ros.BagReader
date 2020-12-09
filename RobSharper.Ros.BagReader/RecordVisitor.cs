using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public class RecordVisitor : IBagRecordVisitor
    {
        public static readonly RecordVisitor NullVisitor = new RecordVisitor();
        
        public virtual void Visit(BagHeader record)
        {
        }

        public virtual void Visit(Chunk record)
        {
        }

        public virtual void Visit(Connection record)
        {
        }

        public virtual void Visit(MessageData record)
        {
        }

        public virtual void Visit(IndexData record)
        {
        }

        public virtual void Visit(ChunkInfo record)
        {
        }

        public virtual void Reset()
        {
        }
    }
}