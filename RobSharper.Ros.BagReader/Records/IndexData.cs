using System;
using System.Collections.Generic;

namespace RobSharper.Ros.BagReader.Records
{
    public class IndexData : IBagRecord
    {
        public const int OpCode = 0x04;

        private readonly Lazy<int> _recordVersion;
        private readonly Lazy<int> _connectionId;
        private readonly Lazy<int> _count;
        private readonly RecordData _rawData;
        private IEnumerable<IndexItem> _data;

        public int RecordVersion => _recordVersion.Value;
        public int ConnectionId => _connectionId.Value;
        public int Count => _count.Value;

        public IEnumerable<IndexItem> Data
        {
            get
            {
                if (_data == null)
                {
                    _data = ReadData();
                }

                return _data;
            }
        }

        public IndexData(RecordHeader header, RecordData data)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            var h = header;
            _rawData = data ?? throw new ArgumentNullException(nameof(data));
            
            _recordVersion = new Lazy<int>(() => h["ver"].ConvertToInt32());
            _connectionId = new Lazy<int>(() => h["conn"].ConvertToInt32());
            _count = new Lazy<int>(() => h["count"].ConvertToInt32());
        }

        public void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }

        private IEnumerable<IndexItem> ReadData()
        {
            if (RecordVersion != 1)
                throw new NotSupportedException($"Record version {RecordVersion} is not supported");

            var items = new List<IndexItem>(Count);
            
            for (var i = 0; i < Count; i++)
            {
                var time = (DateTime)_rawData.Reader.ReadBuiltInType(typeof(DateTime));
                var chunkOffset = _rawData.Reader.ReadInt32();
                var item = new IndexItem(time, chunkOffset);

                items.Add(item);
            }

            return items;
        }
    }
}