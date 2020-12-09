using System;
using System.IO;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public class BagReaderTests : IDisposable
    {
        private FileStream _bagStream;

        private void SetBagFile(string filePath)
        {
            _bagStream = File.OpenRead(filePath);
        }
        
        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_create_bag_reader_for_v2_bag(string bagfile)
        {
            SetBagFile(bagfile);
            
            var rosbag = BagReaderFactory.Create(_bagStream, new NullVisitor());
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_bag(string bagfile)
        {
            SetBagFile(bagfile);
            
            var rosbag = BagReaderFactory.Create(_bagStream, new NullVisitor());
            rosbag.ProcessAll();
        }

        public void Dispose()
        {
            _bagStream?.Dispose();
        }
    }
}