using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using RobSharper.Ros.BagReader.Records;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public class MessageVisitorTests : IDisposable
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
        public void MessageVisitor_stores_all_connections(string bagfile)
        {
            SetBagFile(bagfile);
            var connectionCount = -1;
            
            var visitorMock = new Mock<MessageVisitor>();
            visitorMock.Setup(x => x.Visit(It.IsAny<Connection>())).CallBase();
            visitorMock.Setup(x => x.Visit(It.IsAny<BagHeader>()))
                .Callback<BagHeader>(h =>
                {
                    connectionCount = h.ConnectionCount;
                });
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            reader.ProcessAll();

            Assert.True(connectionCount > 0);
            Assert.Equal(connectionCount, visitorMock.Object.Connections.Count());
        }
        
        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void MessageVisitor_calls_visit_with_message_and_connection(string bagfile)
        {
            SetBagFile(bagfile);
            
            var visitorMock = new Mock<MessageVisitor>();
            visitorMock.Setup(x => x.Visit(It.IsAny<Connection>())).CallBase();
            visitorMock.Setup(x => x.Visit(It.IsAny<MessageData>())).CallBase();
            visitorMock.Setup(x => x.Visit(It.IsAny<MessageData>(), It.IsAny<Connection>()))
                .Callback<MessageData, Connection>((m, c) =>
                {
                    m.Should().NotBeNull();
                    c.Should().NotBeNull();
                    m.ConnectionId.Should().Be(c.ConnectionId);
                });
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            reader.ProcessAll();
        }
        
        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_connection_data_in_message_callback(string bagfile)
        {
            SetBagFile(bagfile);
            
            var visitorMock = new Mock<MessageVisitor>();
            visitorMock.Setup(x => x.Visit(It.IsAny<Connection>())).CallBase();
            visitorMock.Setup(x => x.Visit(It.IsAny<MessageData>())).CallBase();
            visitorMock.Setup(x => x.Visit(It.IsAny<MessageData>(), It.IsAny<Connection>()))
                .Callback<MessageData, Connection>((m, c) =>
                {
                    c.DataTopic.Should().NotBeNull();
                });
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            reader.ProcessAll();
        }
    }
}