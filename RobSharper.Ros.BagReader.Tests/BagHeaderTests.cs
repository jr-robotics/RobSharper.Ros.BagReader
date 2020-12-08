using System;
using System.IO;
using Moq;
using RobSharper.Ros.BagReader.Records;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public class BagHeaderTests : IDisposable
    {
        private readonly FileStream _bagStream;

        public BagHeaderTests()
        {
            _bagStream = File.OpenRead("bags/2019-01-19-16-25-02.bag");
        }

        public void Dispose()
        {
            _bagStream.Dispose();
        }

        [Fact]
        public void Bag_contains_one_header()
        {
            var visitorMock = new Mock<IBagRecordVisitor>();
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
            
            visitorMock.Verify(x => x.Visit(It.IsAny<BagHeader>()), Times.Once);
        }
        

        [Fact]
        public void Bag_header_is_first_record()
        {
            var visitorMock = new Mock<IBagRecordVisitor>();
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessNext();
            
            visitorMock.Verify(x => x.Visit(It.IsAny<BagHeader>()), Times.Once);
        }
    }
}