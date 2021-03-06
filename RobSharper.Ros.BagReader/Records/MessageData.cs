using System;
using System.IO;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader.Records
{
    public class MessageData : RecordBase
    {
        public const int OpCode = 0x02;

        private readonly Lazy<int> _connectionId;
        private readonly Lazy<DateTime> _time;
        private readonly RosBinaryReader _dataReader;

        public int ConnectionId => _connectionId.Value;
        public DateTime Time => _time.Value;

        private byte[] _data;
        private bool _dataRead;

        public MemoryStream Data
        {
            get
            {
                ReadData();
                return new MemoryStream(_data);
            }
        }

        public MessageData(RecordHeader header, RosBinaryReader data) : base(data)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            var h = header;
            _dataReader = data ?? throw new ArgumentNullException(nameof(data));
            
            _connectionId = new Lazy<int>(() => h["conn"].ConvertToInt32());
            _time = new Lazy<DateTime>(() => h["time"].ConvertToDateTime());
        }
        
        public void ReadData()
        {
            if (_dataRead)
                return;

            _data = _dataReader.ReadBytes((int) _dataReader.BaseStream.Length);
            _dataRead = true;
        }

        public override void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}