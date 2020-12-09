using System;
using System.IO;
using RobSharper.Ros.BagReader.Records;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader
{
    
    public class V2BagReader : IBagReader
    {
        private readonly IBagRecordVisitor _visitor;
        private readonly Stream _stream;
        private readonly RosBinaryReader _reader;
        private readonly long _bagStartPosition;

        public V2BagReader(Stream bag, IBagRecordVisitor visitor, bool skipVersionHeader = false)
        {
            if (bag == null) throw new ArgumentNullException(nameof(bag));
            _visitor = visitor ?? NullVisitor.Instance;
            
            if (!skipVersionHeader)
            {
                var version = BagReaderFactory.ReadVersion(bag);

                if (!SupportedRosBagVersions.V2.Equals(version))
                    throw new NotSupportedException("Rosbag version {version} expected");
            }

            _stream = bag;
            _bagStartPosition = bag.Position;
            _reader = new RosBinaryReader(bag);
        }

        public void ProcessAll()
        {
            var hasNext = true;
            while (hasNext)
            {
                hasNext = ProcessNext();
            }
        }

        public void Reset()
        {
            if (!_stream.CanSeek)
                throw new NotSupportedException("Cannot reset underlying stream");

            _stream.Seek(_bagStartPosition, SeekOrigin.Begin);
            _visitor.Reset();
        }

        public bool HasNext()
        {
            return _reader.BaseStream.Position < _reader.BaseStream.Length;
        }
        
        public bool ProcessNext()
        {
            if (!HasNext())
                return false;

            var recordHeader = _reader.ReadBagRecordHeader();
            var recordDataReader = CreateRecordDataReader();

            ProcessRecord(recordHeader, recordDataReader);
            
            return true;
        }

        private RosBinaryReader CreateRecordDataReader()
        {
            var length = _reader.ReadInt32();
            var confinedStream = new ConfinedReadOnlyStream(_stream, length);
            var dataReader = new RosBinaryReader(confinedStream);

            return dataReader;
        }

        private void ProcessRecord(RecordHeader recordHeader, RosBinaryReader recordDataReader)
        {
            IBagRecord record = null;
            
            switch (recordHeader.OpCode)
            {
                case BagHeader.OpCode:
                    record = new BagHeader(recordHeader, recordDataReader);
                    break;
                case Chunk.OpCode:
                    record = new Chunk(recordHeader, recordDataReader);
                    break;
                case Connection.OpCode:
                    record = new Connection(recordHeader, recordDataReader);
                    break;
                case MessageData.OpCode:
                    record = new MessageData(recordHeader, recordDataReader);
                    break;
                case IndexData.OpCode:
                    record = new IndexData(recordHeader, recordDataReader);
                    break;
                case ChunkInfo.OpCode:
                    record = new ChunkInfo(recordHeader, recordDataReader);
                    break;
            }

            if (record != null)
                record.Accept(_visitor);

            record.Dispose();
        }
    }
}