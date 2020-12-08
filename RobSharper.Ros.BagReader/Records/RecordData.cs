using System;
using System.IO;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader.Records
{
    public class RecordData : IDisposable
    {
        private readonly object _lock = new object();
        private byte[] _data;
        
        private readonly RosBinaryReader _reader;

        public byte[] RawData
        {
            get
            {
                if (_data == null)
                    Fetch();
                
                return _data;
            }
        }

        public RosBinaryReader Reader => new RosBinaryReader(new MemoryStream(RawData));

        public bool Fetched { get; private set; }

        public bool Skipped { get; private set; }
        
        public bool Disposed { get; private set; }

        public RecordData(RosBinaryReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }
        
        public void Fetch()
        {
            if (Fetched)
                return;
            
            lock (_lock)
            {
                if (Fetched)
                    return;

                if (Skipped)
                    throw new InvalidOperationException("Data already skipped");
                
                CheckDisposed();

                var length = _reader.ReadInt32();
                _data = _reader.ReadBytes(length);

                Fetched = true;
            }
        }

        public void Skip()
        {
            lock (_lock)
            {
                if (Skipped)
                    return;

                if (Fetched)
                    throw new InvalidOperationException("Data already fetched");
                
                CheckDisposed();

                var length = _reader.ReadInt32();
                _reader.SkipBytes(length);

                Skipped = true;
            }
        }

        private void CheckDisposed()
        {
            if (Disposed)
                throw new ObjectDisposedException("Record Data is already disposed");
        }

        public void Dispose()
        {
            if (Disposed)
                return;
            
            lock (_lock)
            {
                Disposed = true;
            }
        }
    }
}