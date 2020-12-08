using System;
using System.Collections;
using System.Collections.Generic;

namespace RobSharper.Ros.BagReader.Records
{
    public class RecordHeader : IReadOnlyDictionary<string, RecordHeaderValue>
    {
        private readonly Dictionary<string, RecordHeaderValue> _recordFields;
        private int _opCode;

        public int OpCode
        {
            get
            {
                if (_opCode == 0)
                {
                    _opCode = (int) this["op"].ConvertToByte();;
                }

                return _opCode;
            }
        }

        public RecordHeaderValue this[string key] => _recordFields[key];

        public IEnumerable<string> Keys => _recordFields.Keys;
        public IEnumerable<RecordHeaderValue> Values => _recordFields.Values;

        public RecordHeader()
        {
            _recordFields = new Dictionary<string, RecordHeaderValue>();
        }

        public bool ContainsKey(string key)
        {
            return _recordFields.ContainsKey(key);
        }

        public bool TryGetValue(string key, out RecordHeaderValue value)
        {
            return _recordFields.TryGetValue(key, out value);
        }

        public void Add(string fieldName, RecordHeaderValue fieldValue)
        {
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));
            if (fieldValue == null) throw new ArgumentNullException(nameof(fieldValue));
            
            _recordFields.Add(fieldName, fieldValue);
        }

        public IEnumerator<KeyValuePair<string, RecordHeaderValue>> GetEnumerator()
        {
            return _recordFields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _recordFields.Count;
    }
}