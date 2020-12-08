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

            var recordHeader = ReadNextRecordHeader();
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
            
            // Move to end of the record implicitly for all records except Chunks
            // (Chunks contain other records as data)
            if (recordHeader.OpCode != Chunk.OpCode)
            {
                MoveToEndOfRecord(recordData);
            }
            else
            {
                if (!recordData.Skipped && !recordData.Fetched)
                {
                    // Read and forget the data length entry.
                    _reader.ReadInt32();
                }
            }
        }

        private void MoveToEndOfRecord(RecordData recordData)
        {
            if (!recordData.Fetched && !recordData.Skipped)
            {
                recordData.Skip();
            }
        }

        private RecordHeader ReadNextRecordHeader()
        {
            var recordHeader = new RecordHeader();
            var recordLength = _reader.ReadInt32();
            var byteCounter = new StreamByteCounter(_reader.BaseStream);

            var fieldBuffer = new byte[256];
            
            while (byteCounter.BytesRead < recordLength)
            {
                var fieldLength = _reader.ReadInt32();
                _reader.Read(fieldBuffer, 0, fieldLength);

                var separatorIndex = Array.IndexOf(fieldBuffer, (byte) '=');
                
                var fieldName = Encoding.ASCII.GetString(fieldBuffer, 0, separatorIndex);
                var fieldValue = new byte[fieldLength - separatorIndex - 1];
                Array.Copy(fieldBuffer, separatorIndex + 1, fieldValue, 0, fieldValue.Length);

                var recordHeaderValue = new RecordHeaderValue(fieldValue);
                recordHeader.Add(fieldName, recordHeaderValue);
            }

            if (byteCounter.BytesRead != recordLength)
            {
                throw new RosbagException($"Expected record length of {recordLength} bytes, but read {byteCounter.BytesRead} bytes.");
            }
            
            return recordHeader;
        }
    }
}