using System;
using System.Collections.Generic;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader.Records
{
    public class IndexData : RecordBase
    {
        public const int OpCode = 0x04;

        private readonly Lazy<int> _recordVersion;
        private readonly Lazy<int> _connectionId;
        private readonly Lazy<int> _count;
        private readonly RosBinaryReader _dataReader;
        private IEnumerable<IndexItem> _data;
        private bool _dataRead;

        public int RecordVersion => _recordVersion.Value;
        public int ConnectionId => _connectionId.Value;
        public int Count => _count.Value;

        public IEnumerable<IndexItem> Data
        {
            get
            {
                ReadData();
                return _data;
            }
        }

        public IndexData(RecordHeader header, RosBinaryReader data) : base(data)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            var h = header;
            _dataReader = data ?? throw new ArgumentNullException(nameof(data));
            
            _recordVersion = new Lazy<int>(() => h["ver"].ConvertToInt32());
            _connectionId = new Lazy<int>(() => h["conn"].ConvertToInt32());
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

            var items = new List<IndexItem>(Count);
            
            for (var i = 0; i < Count; i++)
            {
                var time = (DateTime)_dataReader.ReadBuiltInType(typeof(DateTime));
                var chunkOffset = _dataReader.ReadInt32();
                var item = new IndexItem(time, chunkOffset);

                items.Add(item);
            }

            _data = items;
            _dataRead = true;
        }
    }
}