using System;
using System.IO;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public class BagReaderTests : IDisposable
    {
        private readonly FileStream _bagStream;

        public BagReaderTests()
        {
            _bagStream = File.OpenRead("bags/2019-01-19-16-25-02.bag");
        }
        
        [Fact]
        public void Can_create_bag_reader_for_v2_bag()
        {
            var rosbag = BagReaderFactory.Create(_bagStream, new NullVisitor());
        }

        [Fact]
        public void Can_read_bag()
        {
            var rosbag = BagReaderFactory.Create(_bagStream, new NullVisitor());
            rosbag.ProcessAll();
        }

        public void Dispose()
        {
            _bagStream.Dispose();
        }
    }
}