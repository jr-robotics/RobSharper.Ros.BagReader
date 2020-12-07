using System;
using System.IO;
using RobSharper.Ros.MessageEssentials.Serialization;

namespace RobSharper.Ros.BagReader
{
    public static class RosBinaryReaderExtensions
    {
        public static void SkipBytes(this RosBinaryReader reader, int count)
        {
            if (reader.BaseStream.CanSeek)
            {
                reader.BaseStream.Seek(count, SeekOrigin.Current);
            }
            else
            {
                var buffer = new byte[256];
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