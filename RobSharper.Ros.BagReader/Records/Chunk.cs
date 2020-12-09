using System;
using System.IO;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader.Records
{
    public class Chunk : IBagRecord
    {
        private readonly RosBinaryReader _dataReader;
        public const int OpCode = 0x05;

        private readonly Lazy<string> _compression;
        private readonly Lazy<int> _uncompressedSize;
        private bool _skipped;
        private bool _disposed;

        public string Compression => _compression.Value;
        public int UncompressedSize => _uncompressedSize.Value;

        public Chunk(RecordHeader header, RosBinaryReader dataReader)
        {
            if (header.OpCode != OpCode)
                throw new ArgumentException("Invalid OP code", nameof(header));

            _dataReader = dataReader;

            var h = header;
            _compression = new Lazy<string>(() => h["compression"].ConvertToString());
            _uncompressedSize = new Lazy<int>(() => h["size"].ConvertToInt32());
        }

        public void SkipChunk()
        {
            if (_dataReader.BaseStream.CanSeek)
            {
                _dataReader.BaseStream.Seek(0, SeekOrigin.End);
            }
            else
            {
                var bytesToSkip = _dataReader.BaseStream.Length - _dataReader.BaseStream.Position;
                _dataReader.SkipBytes((int)bytesToSkip);
            }
            
            _skipped = true;
        }
        
        public void Accept(IBagRecordVisitor visitor)
        {
            visitor.Visit(this);
        }

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

            if (disposing)
            {
                _dataReader?.Dispose();
                
                if (!_skipped && !Compression.Equals("none", StringComparison.InvariantCultureIgnoreCase))
                {
                    GC.SuppressFinalize(this);
                    throw new NotSupportedException("Compressed Chunks are not supported");
                }
            }

            _disposed = true;
        }
    }
}