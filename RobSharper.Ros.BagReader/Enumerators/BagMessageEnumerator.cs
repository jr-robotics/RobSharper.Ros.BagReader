using System.IO;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader.Enumerators
{
    public class BagMessageEnumerator : BagEnumeratorBase<BagMessage>
    {
        public BagMessageEnumerator(Stream bagStream) : base(bagStream, new Visitor())
        {
        }

        private class Visitor : MessageVisitor, ICurrentItemBagRecordVisitor<BagMessage>
        {
            public BagMessage Current { get; private set; }

            public override void Visit(MessageData message, Connection connection)
            {
                Current = new BagMessage(message, connection);
            }

            public override void Reset()
            {
                base.Reset();

                Current = null;
            }
        }
    }
}