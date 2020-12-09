using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    internal class HeaderVisitor : RecordVisitor
    {
        public BagHeader Header { get; private set; }

        public override void Visit(BagHeader record)
        {
            Header = record;
        }
    }
}