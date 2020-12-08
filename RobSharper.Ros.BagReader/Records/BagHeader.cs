using System;

namespace RobSharper.Ros.BagReader.Records
{
    public class BagHeader : IBagRecord
    {
        public const int OpCode = 0x03;

        private readonly Lazy<int> _connectionCount;
        private readonly Lazy<int> _chunkCount;
        private readonly Lazy<long> _indexPos;

        public int ConnectionCount => _connectionCount.Value;
        public int ChunkCount => _chunkCount.Value;
        public long IndexPos => _indexPos.Value;
        
        public RecordData Data { get; }

        public BagHeader(RecordHeader header, RecordData data)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            var h = header;
            Data = data ?? throw new ArgumentNullException(nameof(data));
            
            _connectionCount = new Lazy<int>(() => h["conn_count"].ConvertToInt32());
            _chunkCount = new Lazy<int>(() => h["chunk_count"].ConvertToInt32());
            _indexPos = new Lazy<long>(() => h["chunk_count"].ConvertToInt64());
        }

        public void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}