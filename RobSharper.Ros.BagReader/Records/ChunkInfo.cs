using System;
using System.Collections.Generic;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader.Records
{
    public class ChunkInfo : RecordBase
    {
        public const int OpCode = 0x6;

        private readonly Lazy<int> _recordVersion;
        private readonly Lazy<long> _chunkPosition;
        private readonly Lazy<DateTime> _startTime;
        private readonly Lazy<DateTime> _endTime;
        private readonly Lazy<int> _count;
        private readonly RosBinaryReader _dataReader;
        private IEnumerable<ChunkInfoItem> _data;
        private bool _dataRead;

        public int RecordVersion => _recordVersion.Value;
        public long ChunkPosition => _chunkPosition.Value;
        public DateTime StartTime => _startTime.Value;
        public DateTime EndTime => _endTime.Value;
        public int Count => _count.Value;

        public IEnumerable<ChunkInfoItem> Data
        {
            get
            {
                ReadData();
                return _data;
            }
        }

        public ChunkInfo(RecordHeader header, RosBinaryReader data) : base(data)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            var h = header;
            _dataReader = data ?? throw new ArgumentNullException(nameof(data));
            
            _recordVersion = new Lazy<int>(() => h["ver"].ConvertToInt32());
            _chunkPosition = new Lazy<long>(() => h["chunk_pos"].ConvertToInt64());
            _startTime = new Lazy<DateTime>(() => h["start_time"].ConvertToDateTime());
            _endTime = new Lazy<DateTime>(() => h["end_time"].ConvertToDateTime());
            _count = new Lazy<int>(() => h["count"].ConvertToInt32());
        }

        public override void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void ReadData()
        {
            if (_dataRead)
                return;

            if (RecordVersion != 1)
                throw new NotSupportedException($"Record version {RecordVersion} is not supported");

            var items = new List<ChunkInfoItem>(Count);
            
            for (var i = 0; i < Count; i++)
            {
                var conn = _dataReader.ReadInt32();
                var count = _dataReader.ReadInt32();
                var item = new ChunkInfoItem(conn, count);

                items.Add(item);
            }

            _data = items;
            _dataRead = true;
        }
    }
}