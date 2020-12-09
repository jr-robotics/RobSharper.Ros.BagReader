using System;
using System.IO;
using FluentAssertions;
using Moq;
using RobSharper.Ros.BagReader.Records;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public class ChunkTests : IDisposable
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
        public void Bag_contains_at_least_chunk(string bagfile)
        {
            SetBagFile(bagfile);
            var visitorMock = new Mock<IBagRecordVisitor>();
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
            
            visitorMock.Verify(x => x.Visit(It.IsAny<Chunk>()), Times.AtLeastOnce);
        }
        
        private void VisitorCallbackTest(Action<Chunk> action)
        {
            var visitorMock = new Mock<IBagRecordVisitor>();
            visitorMock.Setup(x => x.Visit(It.IsAny<Chunk>()))
                .Callback<Chunk>(action);
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_Compression(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(chunk => chunk.Compression.Should().BeOneOf("none", "bz2"));
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_UncompressedSize(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(chunk => chunk.UncompressedSize.Should().BeGreaterThan(0));
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_skip_chunk(string bagfile)
        {
            SetBagFile(bagfile);
            var visitorMock = new Mock<IBagRecordVisitor>();
            visitorMock.Setup(x => x.Visit(It.IsAny<Chunk>()))
                .Callback<Chunk>(chunk =>
                {
                    chunk.SkipChunk();
                });
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
            
            visitorMock.Verify(x => x.Visit(It.IsAny<MessageData>()), Times.Never);
        }
    }
}