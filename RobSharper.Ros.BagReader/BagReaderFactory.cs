using System;
using System.IO;
using System.Text;
using RobSharper.Ros.MessageEssentials.Serialization;

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
            
            var versionHeader = bag.ReadBytes(13);
            var versionText = Encoding.ASCII.GetString(versionHeader);

            return new RosBagVersion(versionText);
        }
    }
}