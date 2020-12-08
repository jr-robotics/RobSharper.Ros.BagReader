using System;
using System.IO;
using System.Text;
using RobSharper.Ros.BagReader.Records;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader
{
    public static class RosBinaryReaderExtensions
    {
        private const int DefaultBufferSize = 4096;
        
        public static void SkipBytes(this RosBinaryReader reader, int count)
        {
            if (reader.BaseStream.CanSeek)
            {
                reader.BaseStream.Seek(count, SeekOrigin.Current);
            }
            else
            {
                // Read and forget, if seeking is no option.
                var buffer = count < DefaultBufferSize ? new byte[count] : new byte[DefaultBufferSize];
                var remaining = count;

                while (remaining > 0)
                {
                    var skip = Math.Max(remaining, buffer.Length);
                    remaining -= skip;
                    
                    reader.BaseStream.Read(buffer, 0, skip);
                }
            }
        }

        public static RecordHeader ReadBagRecordHeader(this RosBinaryReader reader)
        {
            var headerLength = reader.ReadInt32();
            return reader.ReadBagRecordHeader(headerLength);
        }
        
        public static RecordHeader ReadBagRecordHeader(this RosBinaryReader reader, int headerLength)
        {
            const int initialBufferSize = 64;
            
            var recordHeader = new RecordHeader();
            var byteCounter = new StreamByteCounter(reader.BaseStream);
            var fieldBuffer = new byte[initialBufferSize];
            
            while (byteCounter.BytesRead < headerLength)
            {
                var fieldLength = reader.ReadInt32();
                
                if (fieldLength > fieldBuffer.Length)
                    fieldBuffer = new byte[fieldLength];
                
                reader.Read(fieldBuffer, 0, fieldLength);

                var separatorIndex = Array.IndexOf(fieldBuffer, (byte) '=');
                
                var fieldName = Encoding.ASCII.GetString(fieldBuffer, 0, separatorIndex);
                var fieldValue = new byte[fieldLength - separatorIndex - 1];
                Array.Copy(fieldBuffer, separatorIndex + 1, fieldValue, 0, fieldValue.Length);

                var recordHeaderValue = new RecordHeaderValue(fieldValue);
                recordHeader.Add(fieldName, recordHeaderValue);
            }

            if (byteCounter.BytesRead != headerLength)
            {
                throw new RosbagException($"Expected record length of {headerLength} bytes, but read {byteCounter.BytesRead} bytes.");
            }
            
            return recordHeader;
        }
    }
}