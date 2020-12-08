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
        private readonly FileStream _bagStream;

        public ChunkTests()
        {
            _bagStream = File.OpenRead("bags/2019-01-19-16-25-02.bag");
        }

        public void Dispose()
        {
            _bagStream.Dispose();
        }

        [Fact]
        public void Bag_contains_one_chunk()
        {
            var visitorMock = new Mock<IBagRecordVisitor>();
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
            
            visitorMock.Verify(x => x.Visit(It.IsAny<Chunk>()), Times.Once);
        }
        
        private void VisitorCallbackTest(Action<Chunk> action)
        {
            var visitorMock = new Mock<IBagRecordVisitor>();
            visitorMock.Setup(x => x.Visit(It.IsAny<Chunk>()))
                .Callback<Chunk>(action);
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
        }

        [Fact]
        public void Can_read_Compression()
        {
            VisitorCallbackTest(chunk => chunk.Compression.Should().BeOneOf("none", "bz2"));
        }

        [Fact]
        public void Can_read_UncompressedSize()
        {
            VisitorCallbackTest(chunk => chunk.UncompressedSize.Should().BeGreaterThan(0));
        }

        [Fact]
        public void Can_skip_chunk()
        {
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