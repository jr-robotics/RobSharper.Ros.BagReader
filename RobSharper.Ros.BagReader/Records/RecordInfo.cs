using System;

namespace RobSharper.Ros.BagReader.Records
{
    public class RecordInfo
    {
        public RecordHeader Header { get; }
        public int DataLength { get; }
        public long DataStart { get; }

        public RecordInfo(RecordHeader header, int dataLength, long dataStart)
        {
            if (dataLength <= 0) throw new ArgumentOutOfRangeException(nameof(dataLength));
            if (dataStart <= 0) throw new ArgumentOutOfRangeException(nameof(dataStart));
            
            Header = header ?? throw new ArgumentNullException(nameof(header));
            DataLength = dataLength;
            DataStart = dataStart;
        }
    }
}