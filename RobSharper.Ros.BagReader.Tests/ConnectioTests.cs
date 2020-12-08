using System;
using System.IO;
using Moq;
using RobSharper.Ros.BagReader.Records;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public class ConnectioTests : IDisposable
    {
        private readonly FileStream _bagStream;

        public ConnectioTests()
        {
            _bagStream = File.OpenRead("bags/2019-01-19-16-25-02.bag");
        }

        public void Dispose()
        {
            _bagStream.Dispose();
        }

        [Fact]
        public void Bag_contains_connections()
        {
            var visitorMock = new Mock<IBagRecordVisitor>();
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
            
            visitorMock.Verify(x => x.Visit(It.IsAny<Connection>()), Times.AtLeastOnce);
        }
    }
}