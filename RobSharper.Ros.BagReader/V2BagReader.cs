using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RobSharper.Ros.BagReader.Records;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader
{
    public class V2BagReader : IBagReader
    {    
        private RosBinaryReader _reader;
        
        public V2BagReader(Stream bag, bool skipVersionHeader = false)
        {
            if (!skipVersionHeader)
            {
                var version = BagReaderFactory.ReadVersion(bag);

                if (!SupportedRosBagVersions.V2.Equals(version))
                    throw new NotSupportedException("Rosbag version {version} expected");
            }

            _reader = new RosBinaryReader(bag);

            ReadHeaderRecord();
        }

        private BagHeader ReadHeaderRecord()
        {
            var header = ReadRecordHeader();
             
            var data = (byte[])null; // TODO

            var bagHeader = new BagHeader(header);
            return bagHeader;
        }

        private RecordHeader ReadRecordHeader()
        {
            var recordFields = new Dictionary<string, byte[]>();
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
                
                recordFields.Add(fieldName, fieldValue);
            }

            if (byteCounter.BytesRead != recordLength)
            {
                throw new RosbagException($"Expected record length of {recordLength} bytes, but read {byteCounter.BytesRead} bytes.");
            }

            var recordHeader = new RecordHeader(recordFields);
            return recordHeader;
        }
    }
}