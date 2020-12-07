using System;
using System.IO;

namespace RobSharper.Ros.BagReader
{
    public class BagReaderFactory
    {
        public static IBagReader Create(Stream bag)
        {
            if (bag == null) throw new ArgumentNullException(nameof(bag));

            var version = ReadVersion(bag);

            if (SupportedRosBagVersions.V2.Equals(version))
            {
                return new V2BagReader(bag, true);
            }
            
            throw new NotSupportedException($"Rosbag version {version} is not supported");
        }

        public static RosBagVersion ReadVersion(Stream bag)
        {
            if (bag == null) throw new ArgumentNullException(nameof(bag));
            
            var reader = new StreamReader(bag);
            var line = reader.ReadLine()?.Trim();

            return new RosBagVersion(line);
        }
    }
}