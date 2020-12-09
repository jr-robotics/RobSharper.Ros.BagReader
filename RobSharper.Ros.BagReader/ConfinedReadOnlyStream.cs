using System;
using System.IO;

namespace RobSharper.Ros.BagReader
{
    public sealed class ConfinedReadOnlyStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly long _startPosition;
        private long _endPosition;
        private long _length;
        private bool _disposed;

        public override bool CanRead
        {
            get
            {
                if (_disposed) return false;
                return _innerStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                if (_disposed) return false;
                return _innerStream.CanSeek;
            }
        }

        public override bool CanWrite => false;
        public override long Length => _length;

        public override long Position
        {
            get
            {
                CheckDisposed();
                return _innerStream.Position - _startPosition;
            }
            set
            {
                CheckDisposed();
                var newPosition = _innerStream.Position + value;
                
                if (newPosition > _endPosition)
                    throw new EndOfStreamException();

                _innerStream.Position = newPosition;
            }
        }

        public override int ReadTimeout
        {
            get => _innerStream.ReadTimeout;
            set => _innerStream.ReadTimeout = value;
        }

        public override bool CanTimeout
        {
            get
            {
                if (_disposed) return false;
                return _innerStream.CanTimeout;
            }
        }

        public ConfinedReadOnlyStream(Stream innerStream, long length)
        {
            _innerStream = innerStream;
            _startPosition = innerStream.Position;
            
            SetLengthInternal(length);
        }
        
        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            var expectedInnerStreamPosition = _innerStream.Position + count;

            if (expectedInnerStreamPosition > _endPosition)
                throw new EndOfStreamException();

            return _innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckDisposed();
            long newInnerStreamPosition;
            
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newInnerStreamPosition = _startPosition + offset;
                    break;
                case SeekOrigin.Current:
                    newInnerStreamPosition = _innerStream.Position + offset;
                    break;
                case SeekOrigin.End:
                    newInnerStreamPosition = _endPosition + offset;
                    break;
                default:
                    throw new NotSupportedException($"Invalid SeekOrigin {origin}");
            }

            if (newInnerStreamPosition > _endPosition)
                throw new IOException("Seek after end of stream");
            
            if (newInnerStreamPosition < _startPosition)
                throw new IOException("Seek before begin of stream");

            _innerStream.Seek(offset, origin);
            return Position;
        }
        
        public override void SetLength(long value)
        {
            throw new NotSupportedException("Stream is read-only");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Stream is read-only");
        }

        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(null, "Stream is closed");
        }
        
        protected override void Dispose(bool disposing)
        {
            _disposed = true;
            base.Dispose(disposing);
        }

        private void SetLengthInternal(long value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            CheckDisposed();
            
            var newEndPosition = _startPosition + value;

            if (newEndPosition > _innerStream.Length)
                throw new EndOfStreamException();

            _endPosition = newEndPosition;
            _length = value;
        }
    }
}