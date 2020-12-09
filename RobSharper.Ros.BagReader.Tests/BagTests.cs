using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public abstract class BagTests
    {
        protected abstract IBag CreateBag(string bagfile);

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_bag_header(string bagfile)
        {
            var bag = CreateBag(bagfile);

            bag.Header.Should().NotBeNull();
        }
        
        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_bag_messages(string bagfile)
        {
            var bag = CreateBag(bagfile);

            bag.Messages.Count().Should().BeGreaterThan(0);
        }
        
        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Can_read_bag_connections(string bagfile)
        {
            var bag = CreateBag(bagfile);

            bag.Connections.Count().Should().BeGreaterThan(0);
        }
    }

    public class FileBagTests : BagTests
    {
        protected override IBag CreateBag(string bagfile)
        {
            return new FileBag(bagfile);
        }
    }

    public class StreamBagTests : BagTests, IDisposable
    {
        private FileStream _stream;

        protected override IBag CreateBag(string bagfile)
        {
            _stream = File.OpenRead(bagfile);

            return new StreamBag(_stream);
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
    
    public class BufferBagTests : BagTests
    {
        protected override IBag CreateBag(string bagfile)
        {
            return BufferBag.Create(bagfile);
        }
    }
}