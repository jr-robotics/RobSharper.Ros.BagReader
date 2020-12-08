using System;

namespace RobSharper.Ros.BagReader.Records
{
    public class Connection : IBagRecord
    {
        public const int OpCode = 0x07;

        private readonly Lazy<int> _connectionId;
        private readonly Lazy<string> _topic;

        public int ConnectionId => _connectionId.Value;
        public string HeaderTopic => _topic.Value;
        
        public RecordData Data { get; }

        public Connection(RecordHeader header, RecordData data)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            var h = header;
            Data = data ?? throw new ArgumentNullException(nameof(data));
            
            _connectionId = new Lazy<int>(() => h["conn"].ConvertToInt32());
            _topic = new Lazy<string>(() => h["topic"].ConvertToString());
        }

        public void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}