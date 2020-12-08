using System;

namespace RobSharper.Ros.BagReader.Records
{
    public class MessageData : IBagRecord
    {
        public const int OpCode = 0x02;

        private readonly Lazy<int> _connectionId;
        private readonly Lazy<DateTime> _time;

        public int ConnectionId => _connectionId.Value;
        public DateTime Time => _time.Value;
        
        public RecordData Data { get; }

        public MessageData(RecordHeader header, RecordData data)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            var h = header;
            Data = data ?? throw new ArgumentNullException(nameof(data));
            
            _connectionId = new Lazy<int>(() => h["conn"].ConvertToInt32());
            _time = new Lazy<DateTime>(() => h["time"].ConvertToDateTime());
        }

        public void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}