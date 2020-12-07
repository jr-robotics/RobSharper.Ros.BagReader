using System;

namespace RobSharper.Ros.BagReader.Records
{
    public class BagHeader
    {
        public const int OpCode = 0x03;
        
        private readonly RecordHeader _header;
        
        private readonly Lazy<int> _connectionCount;
        private readonly Lazy<int> _chunkCount;
        private readonly Lazy<long> _indexPos;

        public int ConnectionCount => _connectionCount.Value;
        public int ChunkCount => _chunkCount.Value;
        private long IndexPos => _indexPos.Value;

        public BagHeader(RecordHeader header)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            _header = header;
            _connectionCount = new Lazy<int>(() => _header.GetInt32Field("conn_count"));
            _chunkCount = new Lazy<int>(() => _header.GetInt32Field("chunk_count"));
            _indexPos = new Lazy<long>(() => _header.GetInt64Field("chunk_count"));
        }
    }
}