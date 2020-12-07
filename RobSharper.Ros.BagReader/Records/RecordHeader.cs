using System;
using System.Collections.Generic;
using System.IO;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader.Records
{
    public class RecordHeader
    {
        private readonly Dictionary<string, byte[]> _recordFields;
        private int _opCode;

        public int OpCode
        {
            get
            {
                if (_opCode == 0)
                {
                    _opCode = (int)GetByteField("op");
                }

                return _opCode;
            }
        }

        public RecordHeader(Dictionary<string,byte[]> recordFields)
        {
            _recordFields = recordFields ?? throw new ArgumentNullException(nameof(recordFields));
        }

        public RosBinaryReader GetFieldReader(string id)
        {
            var value = _recordFields[id];
            var reader = new RosBinaryReader(new MemoryStream(value));
            return reader;
        }

        public byte GetByteField(string id)
        {
            using (var r = GetFieldReader(id))
            {
                return r.ReadByte();
            }
        }

        public int GetInt32Field(string id)
        {
            using (var r = GetFieldReader(id))
            {
                return r.ReadInt32();
            }
        }

        public long GetInt64Field(string id)
        {
            using (var r = GetFieldReader(id))
            {
                return r.ReadInt64();
            }
        }
    }
}