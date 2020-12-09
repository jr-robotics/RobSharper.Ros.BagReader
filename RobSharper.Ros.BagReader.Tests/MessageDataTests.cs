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
        public void Bag_contains_messages(string bagfile)
        {
            SetBagFile(bagfile);
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

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_ConnectionId(string bagfile)
        {
            SetBagFile(bagfile);
            VisitorCallbackTest(messageData => messageData.ConnectionId.Should().BeInRange(int.MinValue, int.MaxValue));
        }
        
        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void All_Timestamps_must_be_after_bagdate(string bagfile)
        {
            SetBagFile(bagfile);
            var expectedMinDate = new DateTime(2019,01,19,15,25,0);

            VisitorCallbackTest(m =>
            {
                m.Time.Should().BeAfter(expectedMinDate, "messages should be after 2019-01-19 15:25:00");
            });
        }
        
        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Messages_are_sorted_by_time(string bagfile)
        {
            SetBagFile(bagfile);
            var lastTimestamp = DateTime.MinValue;

            VisitorCallbackTest(m =>
            {
                m.Time.Should().BeOnOrAfter(lastTimestamp, "messages should be ordered by time");
                lastTimestamp = m.Time;
            });
        }
    }
}