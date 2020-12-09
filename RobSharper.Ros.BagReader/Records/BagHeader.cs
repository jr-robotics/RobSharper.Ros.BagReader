using System;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader.Records
{
    public abstract class RecordBase : IBagRecord
    {
        private readonly RosBinaryReader _recordData;
        private bool _disposed;

        public RecordBase(RosBinaryReader recordData)
        {
            _recordData = recordData;
        }

        public abstract void Accept(IBagRecordVisitor visitor);
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing && _recordData != null)
            {
                var bytesToSkip = _recordData.BaseStream.Length - _recordData.BaseStream.Position;
                _recordData.SkipBytes((int)bytesToSkip);
                _recordData?.Dispose();
            }

            _disposed = true;
        }
    }
    public class BagHeader : RecordBase
    {
        public const int OpCode = 0x03;

        private readonly Lazy<int> _connectionCount;
        private readonly Lazy<int> _chunkCount;
        private readonly Lazy<long> _indexPos;

        public int ConnectionCount => _connectionCount.Value;
        public int ChunkCount => _chunkCount.Value;
        public long IndexPos => _indexPos.Value;
        
        public RosBinaryReader DataReader { get; }

        public BagHeader(RecordHeader header, RosBinaryReader dataReader) : base(dataReader)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));
            
            var h = header;
            DataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
            
            _connectionCount = new Lazy<int>(() => h["conn_count"].ConvertToInt32());
            _chunkCount = new Lazy<int>(() => h["chunk_count"].ConvertToInt32());
            _indexPos = new Lazy<long>(() => h["index_pos"].ConvertToInt64());
        }

        public override void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}