using System.IO;
using FluentAssertions;
using Xunit;

namespace RobSharper.Ros.BagReader.Tests
{
    public class BagMessageEnumeratorTests
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
        public void Can_enumerate_over_bag_messages(string bagfile)
        {
            SetBagFile(bagfile);

            var enumerator = new BagMessageEnumerator(_bagStream);

            while (enumerator.MoveNext())
            {
                enumerator.Current.Should().NotBeNull();
            }
        }

        [Theory]
        [ClassData(typeof(Bagfiles.All))]
        public void Bag_contains_messages(string bagfile)
        {
            SetBagFile(bagfile);

            var enumerator = new BagMessageEnumerator(_bagStream);

            var count = 0;
            while (enumerator.MoveNext())
            {
                count++;
            }

            count.Should().BeGreaterThan(0);
        }
    }
}