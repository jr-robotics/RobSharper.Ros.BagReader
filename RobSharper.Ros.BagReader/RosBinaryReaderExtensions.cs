using System;
using System.IO;
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
    }
}