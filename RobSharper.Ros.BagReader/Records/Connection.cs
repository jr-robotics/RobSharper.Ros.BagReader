using System;
using RobSharper.Ros.MessageEssentials;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader.Records
{
    public class Connection : RecordBase
    {
        public const int OpCode = 0x07;

        private readonly Lazy<int> _connectionId;
        private readonly Lazy<string> _headerTopic;
        private readonly RosBinaryReader _data;
        
        private bool _dataRead;
        private string _dataTopic;
        private RosType _type;
        private string _md5Sum;
        private string _messageDefinition;
        private string _callerId;
        private bool _latching;

        public int ConnectionId => _connectionId.Value;
        public string HeaderTopic => _headerTopic.Value;

        
        public string DataTopic
        {
            get
            {
                ReadData();
                return _dataTopic;
            }
        }

        public RosType Type
        {
            get
            {
                ReadData();
                return _type;
            }
        }

        public string MD5Sum
        {
            get
            {
                ReadData();
                return _md5Sum;
            }
        }

        public string MessageDefinition
        {
            get
            {
                ReadData();
                return _messageDefinition;
            }
        }

        public string CallerId
        {
            get
            {
                ReadData();
                return _callerId;
            }
        }

        public bool Latching
        {
            get
            {
                ReadData();
                return _latching;
            }
        }


        public Connection(RecordHeader header, RosBinaryReader data) : base(data)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            var h = header;
            _data = data;

            _connectionId = new Lazy<int>(() => h["conn"].ConvertToInt32());
            _headerTopic = new Lazy<string>(() => h["topic"].ConvertToString());
        }
        
        public void ReadData()
        {
            if (_dataRead)
                return;
            
            var values = _data.ReadBagRecordHeader((int)_data.BaseStream.Length);

            _dataTopic = values["topic"].ConvertToString();
            _type = RosType.Parse(values["type"].ConvertToString());
            _md5Sum = values["md5sum"].ConvertToString();
            _messageDefinition = values["message_definition"].ConvertToString();

            if (values.ContainsKey("callerid"))
            {
                _callerId = values["callerid"].ConvertToString();
            }
            
            if (values.ContainsKey("latching"))
            {
                _latching = values["latching"].ConvertToByte() == 1;
            }

            _dataRead = true;
        }

        public override void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}