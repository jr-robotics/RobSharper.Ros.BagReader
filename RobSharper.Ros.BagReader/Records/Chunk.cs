using System;

namespace RobSharper.Ros.BagReader.Records
{
    public class Chunk : IBagRecord
    {
        private readonly RecordData _recordData;
        public const int OpCode = 0x05;

        private readonly Lazy<string> _compression;
        private readonly Lazy<int> _uncompressedSize;

        public string Compression => _compression.Value;
        public int UncompressedSize => _uncompressedSize.Value;

        public Chunk(RecordHeader header, RecordData recordData)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));

            _recordData = recordData;

            var h = header;
            _compression = new Lazy<string>(() => h["compression"].ConvertToString());
            _uncompressedSize = new Lazy<int>(() => h["size"].ConvertToInt32());
        }

        public void SkipChunk()
        {
            _recordData.Skip();
        }
        
        public void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}