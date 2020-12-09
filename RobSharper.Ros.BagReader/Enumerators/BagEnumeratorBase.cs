using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace RobSharper.Ros.BagReader.Enumerators
{
    public abstract class BagEnumeratorBase<T> : IEnumerator<T> where T : class
    {
        private bool _disposed;
        private readonly IBagReader _bagReader;
        private readonly ICurrentItemBagRecordVisitor<T> _visitor;
        
        private T _current;

        public BagEnumeratorBase(Stream bagStream, ICurrentItemBagRecordVisitor<T> visitor)
        {
            if (bagStream == null) throw new ArgumentNullException(nameof(bagStream));

            _visitor = visitor;
            _bagReader = BagReaderFactory.Create(bagStream, _visitor);
        }

        public T Current
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(null);

                return _current;
            }
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            
            while (_bagReader.ProcessNext())
            {
                if (Current != _visitor.Current)
                {
                    _current = _visitor.Current;
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            
            _bagReader.Reset();
            _current = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }
    }
}