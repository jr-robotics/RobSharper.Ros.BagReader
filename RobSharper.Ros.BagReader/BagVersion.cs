using System;

namespace RobSharper.Ros.BagReader
{
    public class RosBagVersion
    {
        public string Version { get; }
            
        public RosBagVersion(string versionHeader)
        {
            if (versionHeader == null) throw new ArgumentNullException(nameof(versionHeader));
                
            if (!versionHeader.StartsWith("#ROSBAG", StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException($"Invalid version header '{versionHeader}'", nameof(versionHeader));
            
            Version = versionHeader.Trim();
        }
        
        public bool Match(string versionHeader)
        {
            if (versionHeader == null) throw new ArgumentNullException(nameof(versionHeader));
                
            return Version.Equals(versionHeader.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as RosBagVersion;

            if (other == null)
                return false;
            
            return Version.Equals(other.Version, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Version.GetHashCode();
        }

        public override string ToString()
        {
            return Version;
        }
    }
}