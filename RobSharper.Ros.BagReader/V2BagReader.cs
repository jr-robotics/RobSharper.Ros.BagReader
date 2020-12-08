using System;
using System.IO;
using System.Text;
using RobSharper.Ros.BagReader.Records;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader
{
    
    public class V2BagReader : IBagReader
    {
        private readonly IBagRecordVisitor _visitor;
        private readonly RosBinaryReader _reader;
        
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

        public bool HasNext()
        {
            return _reader.BaseStream.Position < _reader.BaseStream.Length;
        }
        
        public bool ProcessNext()
        {
            if (!HasNext())
                return false;

            var recordHeader = _reader.ReadBagRecordHeader();
            var recordData = new RecordData(_reader);

            ProcessRecord(recordHeader, recordData);
            
            recordData.Dispose();
            return true;
        }

        private void ProcessRecord(RecordHeader recordHeader, RecordData recordData)
        {
            IBagRecord record = null;
            
            switch (recordHeader.OpCode)
            {
                case BagHeader.OpCode:
                    record = new BagHeader(recordHeader, recordData);
                    break;
                case Chunk.OpCode:
                    record = new Chunk(recordHeader, recordData);
                    break;
                case Connection.OpCode:
                    record = new Connection(recordHeader, recordData);
                    break;
                case MessageData.OpCode:
                    record = new MessageData(recordHeader, recordData);
                    break;
                case IndexData.OpCode:
                    record = new IndexData(recordHeader, recordData);
                    break;
                case ChunkInfo.OpCode:
                    record = new ChunkInfo(recordHeader, recordData);
                    break;
            }

            if (record != null)
                record.Accept(_visitor);
            
            MoveToEndOfRecord(recordHeader, recordData);
        }

        private void MoveToEndOfRecord(RecordHeader recordHeader, RecordData recordData)
        {
            // Move to end of the record implicitly for all records except Chunks
            // (Chunks contain other records as data)
            if (recordHeader.OpCode != Chunk.OpCode)
            {
                if (!recordData.Fetched && !recordData.Skipped)
                {
                    recordData.Skip();
                }
            }
            else
            {
                if (!recordData.Skipped && !recordData.Fetched)
                {
                    // Compressed chunks are not yet supported
                    var chunk = new Chunk(recordHeader, recordData);
                    if (!chunk.Compression.Equals("none", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new NotSupportedException("Compressed Chunks are not supported");
                    }
                    
                    // Read and forget the data length entry.
                    _reader.ReadInt32();
                }
            }
        }
    }
}