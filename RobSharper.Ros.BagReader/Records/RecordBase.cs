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
}