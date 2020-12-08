using System;
using System.IO;
using FluentAssertions;
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
        
        private void VisitorCallbackTest(Action<BagHeader> action)
        {
            var visitorMock = new Mock<IBagRecordVisitor>();
            visitorMock.Setup(x => x.Visit(It.IsAny<BagHeader>()))
                .Callback<BagHeader>(action);
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
        }

        [Fact]
        public void Can_read_ChunkCount()
        {
            VisitorCallbackTest(header => header.ChunkCount.Should().BeGreaterOrEqualTo(0));
        }

        [Fact]
        public void Can_read_ConnectionCount()
        {
            VisitorCallbackTest(header => header.ConnectionCount.Should().BeGreaterOrEqualTo(0));
        }

        [Fact]
        public void Can_read_IndexPos()
        {
            VisitorCallbackTest(header => header.IndexPos.Should().BeGreaterOrEqualTo(0));
        }
    }
}