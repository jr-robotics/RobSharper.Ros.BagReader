using System;
using System.IO;
using FluentAssertions;
using Moq;
using RobSharper.Ros.BagReader.Records;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
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
        
        private void VisitorCallbackTest(Action<MessageData> action)
        {
            var visitorMock = new Mock<IBagRecordVisitor>();
            visitorMock.Setup(x => x.Visit(It.IsAny<MessageData>()))
                .Callback<MessageData>(action);
            
            var reader = BagReaderFactory.Create(_bagStream, visitorMock.Object);
            
            reader.ProcessAll();
        }

        [Fact]
        public void Can_read_ConnectionId()
        {
            VisitorCallbackTest(messageData => messageData.ConnectionId.Should().BeInRange(int.MinValue, int.MaxValue));
        }
        
        [Fact]
        public void All_Timestamps_must_be_after_bagdate()
        {
            var expectedMinDate = new DateTime(2019,01,19,15,25,0);

            VisitorCallbackTest(m =>
            {
                m.Time.Should().BeAfter(expectedMinDate, "messages should be after 2019-01-19 15:25:00");
            });
        }
        
        [Fact]
        public void Messages_are_sorted_by_time()
        {
            var lastTimestamp = DateTime.MinValue;

            VisitorCallbackTest(m =>
            {
                m.Time.Should().BeOnOrAfter(lastTimestamp, "messages should be ordered by time");
                lastTimestamp = m.Time;
            });
        }
    }
}