using System;

namespace RobSharper.Ros.BagReader
{
    public class SupportedRosBagVersions
    {
        public static RosBagVersion V2 = new RosBagVersion("#ROSBAG V2.0");

        public static bool IsSupported(RosBagVersion version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            
            return V2.Equals(version);
        }
    }
}