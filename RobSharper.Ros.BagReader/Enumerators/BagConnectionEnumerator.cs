using System.IO;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader.Enumerators
{
    public sealed class BagConnectionEnumerator : BagEnumeratorBase<Connection>
    {
        public BagConnectionEnumerator(Stream bagStream) : base(bagStream, new Visitor())
        {
        }

        private class Visitor : RecordVisitor, ICurrentItemBagRecordVisitor<Connection>
        {
            public Connection Current { get; private set; }

            public override void Visit(Connection connection)
            {
                Current = connection;
            }

            public override void Reset()
            {
                base.Reset();
                Current = null;
            }
        }
    }
}