using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using RobSharper.Ros.BagReader.Enumerators;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public class StreamBag : IBag
    {
        public BagHeader Header { get; }
        public IEnumerable<Connection> Connections { get; }
        public IEnumerable<BagMessage> Messages { get; }

        public StreamBag(Stream bag)
        {
            if (bag == null) throw new ArgumentNullException(nameof(bag));
            
            if (!bag.CanSeek)
                throw new InvalidOperationException("Cannot seek on stream");

            var mutex = new Mutex();

            Header = ReadHeader(bag, mutex);
            Messages = new MessageCollection(bag, mutex);
            Connections = new ConnectionCollection(bag, mutex);
        }

        private static BagHeader ReadHeader(Stream bag, Mutex mutex)
        {
            var initialPosition = bag.Position;
            
            try
            {
                mutex.WaitOne();
                var headerVisitor = new HeaderVisitor();
                var rosbag = BagReaderFactory.Create(bag, headerVisitor);
                
                do
                {
                    rosbag.ProcessNext();

                    if (headerVisitor.Header != null)
                        break;

                } while (rosbag.HasNext());

                return headerVisitor.Header;
            }
            finally
            {
                bag.Seek(initialPosition, SeekOrigin.Begin);
                mutex.ReleaseMutex();
            }
        }

        private abstract class Collection<T> : IEnumerable<T>
        {
            private readonly Stream _stream;
            private readonly Mutex _mutex;

            internal Collection(Stream stream, Mutex mutex)
            {
                _stream = stream;
                _mutex = mutex;
            }

            public IEnumerator<T> GetEnumerator()
            {
                var enumerator = GetEnumerator(_stream);
                return new StreamResettingMutexEnumerator<T>(enumerator, _stream, _mutex);
            }

            protected abstract IEnumerator<T> GetEnumerator(Stream stream);
            
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        
        private class MessageCollection : Collection<BagMessage>
        {
            internal MessageCollection(Stream stream, Mutex mutex) : base(stream, mutex)
            {
            }

            protected override IEnumerator<BagMessage> GetEnumerator(Stream stream)
            {
                return new BagMessageEnumerator(stream);
            }
        }
        
        private class ConnectionCollection : Collection<Connection>
        {
            internal ConnectionCollection(Stream stream, Mutex mutex) : base(stream, mutex)
            {
            }

            protected override IEnumerator<Connection> GetEnumerator(Stream stream)
            {
                return new BagConnectionEnumerator(stream);
            }
        }

        private class StreamResettingMutexEnumerator<T> : IEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            private readonly Stream _stream;
            private readonly Mutex _mutex;
            private bool _disposed;
            private long _initialPosition;

            public StreamResettingMutexEnumerator(IEnumerator<T> enumerator, Stream stream, Mutex mutex)
            {
                _inner = enumerator;
                _stream = stream;
                _mutex = mutex;

                _mutex.WaitOne();
                _initialPosition = _stream.Position;
            }
            
            public bool MoveNext()
            {
                return _inner.MoveNext();
            }

            public void Reset()
            {
                _inner.Reset();
            }

            public T Current => _inner.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (_disposed)
                    return;

                _stream.Seek(_initialPosition, SeekOrigin.Begin);
                _mutex.ReleaseMutex();
                
                _disposed = true;
            }
        }
    }
}