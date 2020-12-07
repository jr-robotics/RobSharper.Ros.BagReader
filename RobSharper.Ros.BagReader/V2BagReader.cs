using System;
using System.IO;

namespace RobSharper.Ros.BagReader
{
    public class V2BagReader : IBagReader
    {
        public V2BagReader(Stream bag, bool skipVersionHeader = false)
        {
            if (!skipVersionHeader)
            {
                var version = BagReaderFactory.ReadVersion(bag);

                if (!SupportedRosBagVersions.V2.Equals(version))
                    throw new NotSupportedException("Rosbag version {version} expected");
            }
            
            
        }
    }
}