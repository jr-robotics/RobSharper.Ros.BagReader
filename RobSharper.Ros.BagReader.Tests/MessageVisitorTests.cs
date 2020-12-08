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
        private readonly FileStream _bagStream;

        public MessageVisitorTests()
        {
            _bagStream = File.OpenRead("bags/2019-01-19-16-25-02.bag");
        }

        public void Dispose()
        {
            _bagStream.Dispose();
        }

        [Fact]
        public void MessageVisitor_stores_all_connections()
        {
            var connectionCount = -1;
            
            var visitorMock = new Mock<MessageVisitorBase>();
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
        

        [Fact]
        public void MessageVisitor_calls_visit_with_message_and_connection()
        {
            var visitorMock = new Mock<MessageVisitorBase>();
            visitorMock.Setup(x => x.Visit(It.IsAny<Connection>())).CallBase();
            visitorMock.Setup(x => x.Visit(It.IsAny<MessageData>())).CallBase();
            visitorMock.Setup(x => x.Visit(It.IsAny<MessageData>(), It.IsAny<Connection>()))
                .Callback<MessageData, Connection>((m, c) =>
                {
                    Assert.Equal(m.ConnectionId, c.ConnectionId);
                });
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            reader.ProcessAll();
        }
    }
    
    public class MessageDataTests : IDisposable
    {
        private readonly FileStream _bagStream;

        public MessageDataTests()
        {
            _bagStream = File.OpenRead("bags/2019-01-19-16-25-02.bag");
        }

        public void Dispose()
        {
            _bagStream.Dispose();
        }

        [Fact]
        public void Bag_contains_messages()
        {
            var visitorMock = new Mock<IBagRecordVisitor>();
           
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            reader.ProcessAll();
            
            visitorMock.Verify(x => x.Visit(It.IsAny<MessageData>()), Times.AtLeastOnce);
        }
        
        [Fact]
        public void All_Timestamps_must_be_after_bagdate()
        {
            var expectedMinDate = new DateTime(2019,01,19,15,25,0);
            
            var visitorMock = new Mock<MessageVisitorBase>();
            visitorMock.Setup(x => x.Visit(It.IsAny<MessageData>()))
                .Callback<MessageData>(m =>
                {
                    m.Time.Should().BeAfter(expectedMinDate, "messages should be after 2019-01-19 15:25:00");
                });
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            reader.ProcessAll();
        }
        
        [Fact]
        public void Messages_are_sorted_by_time()
        {
            var lastTimestamp = DateTime.MinValue;
            
            var visitorMock = new Mock<MessageVisitorBase>();
            visitorMock.Setup(x => x.Visit(It.IsAny<MessageData>()))
                .Callback<MessageData>(m =>
                {
                    m.Time.Should().BeOnOrAfter(lastTimestamp, "messages should be ordered by time");
                    lastTimestamp = m.Time;
                });
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            reader.ProcessAll();
        }
    }
}