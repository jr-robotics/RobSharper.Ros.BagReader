using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace RobSharper.Ros.BagReader
{
    internal sealed class DisposingStreamEnumerator<TItem> : IEnumerator<TItem>
    {
        private bool _disposed;
        private readonly Stream _stream;
        private readonly IEnumerator<TItem> _inner;

        public DisposingStreamEnumerator(IEnumerator<TItem> enumerator, Stream s)
        {
            _inner = enumerator;
            _stream = s;
        }
        
        public bool MoveNext()
        {
            return _inner.MoveNext();
        }

        public void Reset()
        {
            _inner.Reset();
        }

        public TItem Current => _inner.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            if (_disposed)
                return;
            
            _inner.Dispose();
            _stream.Dispose();
            
            _disposed = true;
        }
    }
}