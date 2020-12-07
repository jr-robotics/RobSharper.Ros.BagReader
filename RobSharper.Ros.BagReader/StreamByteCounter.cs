using System.IO;

namespace RobSharper.Ros.BagReader
{
    internal class StreamByteCounter
    {
        private readonly Stream _stream;
        private readonly long _initialPosition;

        public long BytesRead => _stream.Position - _initialPosition;

        public StreamByteCounter(Stream stream)
        {
            _stream = stream;
            _initialPosition = stream.Position;
        }
    }
}