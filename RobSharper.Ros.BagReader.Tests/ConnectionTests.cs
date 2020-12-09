using System;
using System.IO;
using FluentAssertions;
using Moq;
using RobSharper.Ros.BagReader.Records;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public class ConnectionTests : IDisposable
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
        public void Bag_contains_connections(string bagfile)
        {
            SetBagFile(bagfile);
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

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_MD5Sum(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(connection => connection.MD5Sum.Should().NotBeNullOrEmpty());
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_ConnectionId(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(connection => connection.ConnectionId.Should().BeInRange(int.MinValue, int.MaxValue));
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_HeaderTopic(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(connection => connection.HeaderTopic.Should().NotBeNullOrEmpty());
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_DataTopic(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(connection => connection.DataTopic.Should().NotBeNullOrEmpty());
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_Type(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(connection => connection.Type.Should().NotBeNull());
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_MessageDefinition(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(connection => connection.MessageDefinition.Should().NotBeNullOrEmpty());
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_CallerId(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(connection =>
            {
                var optionalCallerId = connection.CallerId;
                Assert.True(optionalCallerId == null || optionalCallerId != string.Empty);
            });
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_Latching(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(connection => Assert.True(connection.Latching == true || connection.Latching == false));
        }
    }
}