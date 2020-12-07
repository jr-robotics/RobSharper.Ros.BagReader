using System.IO;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public class BagReaderTests
    {
        [Fact]
        public void Can_create_bag_reader_for_v2_bag()
        {
            using (var fs = File.OpenRead("bags/2019-01-19-16-25-02.bag"))
            {
                var rosbag = BagReaderFactory.Create(fs);
            }
        }
    }
}