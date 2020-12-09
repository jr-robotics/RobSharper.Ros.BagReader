using System;
using System.IO;
using System.Text;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader.Records
{
    public class RecordHeaderValue
    {
        private readonly Lazy<byte[]> _littleEndianData;
        private readonly byte[] _data;

        public byte[] Data => _data;
        
        public RecordHeaderValue(byte[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _littleEndianData = new Lazy<byte[]>(() => _data.ToLittleEndian());
        }

        public string ConvertToString()
        {
            return Encoding.ASCII.GetString(Data);
        }

        public byte ConvertToByte()
        {
            if (Data.Length != 1)
                throw new InvalidCastException();
            
            return Data[0];
        }

        public int ConvertToInt32()
        {
            if (Data.Length != 4)
                throw new InvalidCastException();
            
            return BitConverter.ToInt32(_littleEndianData.Value, 0);
        }

        public long ConvertToInt64()
        {
            if (Data.Length != 8)
                throw new InvalidCastException();
            
            return BitConverter.ToInt64(_littleEndianData.Value, 0);
        }

        public DateTime ConvertToDateTime()
        {
            var secs = BitConverter.ToInt32(_littleEndianData.Value, 0);
            var nsecs = BitConverter.ToInt32(_littleEndianData.Value, 4);

            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)
                .AddSeconds(secs)
                .AddMilliseconds(nsecs / 1000000.0);

            return dateTime;
        }
    }
}