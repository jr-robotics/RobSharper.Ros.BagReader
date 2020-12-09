using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using RobSharper.Ros.BagReader.Records;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public class BagHeaderTests : IDisposable
    {
        private FileStream _bagStream;

        private void SetBagFile(string filePath)
        {
            _bagStream = File.OpenRead(filePath);
        }

        public void Dispose()
        {
            _bagStream?.Dispose();
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Bag_contains_one_header(string bagfile)
        {
            SetBagFile(bagfile);
            var visitorMock = new Mock<IBagRecordVisitor>();
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
            
            visitorMock.Verify(x => x.Visit(It.IsAny<BagHeader>()), Times.Once);
        }
        
        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Bag_header_is_first_record(string bagfile)
        {
            SetBagFile(bagfile);
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

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_ChunkCount(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(header => header.ChunkCount.Should().BeGreaterOrEqualTo(0));
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_ConnectionCount(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(header => header.ConnectionCount.Should().BeGreaterOrEqualTo(0));
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_IndexPos(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(header => header.IndexPos.Should().BeGreaterOrEqualTo(0));
        }
    }
}