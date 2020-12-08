using System;

namespace RobSharper.Ros.BagReader.Records
{
    public class Connection : IBagRecord
    {
        public const int OpCode = 0x07;

        private readonly Lazy<int> _connectionId;
        private readonly Lazy<string> _headerTopic;
        private readonly RecordData _rawData;
        
        private bool _dataRead;
        private string _dataTopic;
        private string _type;
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

        public string Type
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


        public Connection(RecordHeader header, RecordData data)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            var h = header;
            _rawData = data;

            _connectionId = new Lazy<int>(() => h["conn"].ConvertToInt32());
            _headerTopic = new Lazy<string>(() => h["topic"].ConvertToString());
        }
        
        private void ReadData()
        {
            if (_dataRead)
                return;
            
            var values = _rawData.Reader.ReadBagRecordHeader(_rawData.RawData.Length);

            _dataTopic = values["topic"].ConvertToString();
            _type = values["type"].ConvertToString();
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

        public void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}