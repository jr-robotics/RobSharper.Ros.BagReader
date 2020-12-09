using System;
using System.IO;
using Moq;
using RobSharper.Ros.BagReader.Records;
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
            
            var rosbag = BagReaderFactory.Create(_bagStream, RecordVisitor.NullVisitor);
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_bag(string bagfile)
        {
            SetBagFile(bagfile);
            
            var rosbag = BagReaderFactory.Create(_bagStream, RecordVisitor.NullVisitor);
            rosbag.ProcessAll();
        }


        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_reset_reader(string bagfile)
        {
            SetBagFile(bagfile);
            
            var visitorMock = new Mock<IBagRecordVisitor>();
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessNext();
            reader.Reset();
            reader.ProcessNext();
            
            // The first record is a BagHeader and a bag contains only one BagHeader
            visitorMock.Verify(x => x.Visit(It.IsAny<BagHeader>()), Times.Exactly(2), "Visit(BagHeader) should be called");
            visitorMock.Verify(x => x.Reset(), Times.Once, "Reset should be called.");
            visitorMock.VerifyNoOtherCalls();
        }

        public void Dispose()
        {
            _bagStream?.Dispose();
        }
    }
}