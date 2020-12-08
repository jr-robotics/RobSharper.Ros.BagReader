using System;

namespace RobSharper.Ros.BagReader.Records
{
    [Obsolete]
    public class RecordInfo
    {
        public RecordHeader Header { get; }
        public long RecordStartPosition { get; }
        public long DataStartPosition { get; }

        public RecordInfo(RecordHeader header, long recordStartPosition, long dataStartPosition)
        {
            if (recordStartPosition <= 0) throw new ArgumentOutOfRangeException(nameof(recordStartPosition));
            if (dataStartPosition <= 0) throw new ArgumentOutOfRangeException(nameof(dataStartPosition));
            
            Header = header ?? throw new ArgumentNullException(nameof(header));
            RecordStartPosition = recordStartPosition;
            DataStartPosition = dataStartPosition;
        }
    }
}