using System;
using System.IO;
using FluentAssertions;
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
        
        private void VisitorCallbackTest(Action<Connection> action)
        {
            var visitorMock = new Mock<IBagRecordVisitor>();
            visitorMock.Setup(x => x.Visit(It.IsAny<Connection>()))
                .Callback<Connection>(action);
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
        }

        [Fact]
        public void Can_read_MD5Sum()
        {
            VisitorCallbackTest(connection => connection.MD5Sum.Should().NotBeNullOrEmpty());
        }

        [Fact]
        public void Can_read_ConnectionId()
        {
            VisitorCallbackTest(connection => connection.ConnectionId.Should().BeInRange(int.MinValue, int.MaxValue));
        }

        [Fact]
        public void Can_read_HeaderTopic()
        {
            VisitorCallbackTest(connection => connection.HeaderTopic.Should().NotBeNullOrEmpty());
        }

        [Fact]
        public void Can_read_DataTopic()
        {
            VisitorCallbackTest(connection => connection.DataTopic.Should().NotBeNullOrEmpty());
        }

        [Fact]
        public void Can_read_Type()
        {
            VisitorCallbackTest(connection => connection.Type.Should().NotBeNullOrEmpty());
        }

        [Fact]
        public void Can_read_MessageDefinition()
        {
            VisitorCallbackTest(connection => connection.MessageDefinition.Should().NotBeNullOrEmpty());
        }

        [Fact]
        public void Can_read_CallerId()
        {
            VisitorCallbackTest(connection =>
            {
                var optionalCallerId = connection.CallerId;
                Assert.True(optionalCallerId == null || optionalCallerId != string.Empty);
            });
        }

        [Fact]
        public void Can_read_Latching()
        {
            VisitorCallbackTest(connection => Assert.True(connection.Latching == true || connection.Latching == false));
        }
    }
}