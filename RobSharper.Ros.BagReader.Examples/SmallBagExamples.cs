using System.IO;
using FluentAssertions;
using Xunit;

namespace RobSharper.Ros.BagReader.Examples
{
    public class SmallBagExamples
    {
        [Fact]
        public void Read_bag_from_file()
        {
            /*
             * Create am IBag from a rosbag file.
             *
             * The BufferBag reads the whole bag in the Create method.
             * If you work with large bags consider using FileBag or StreamBag insterd.
             */
            var bag = BufferBag.Create(ExampleBag.FilePath);

            /*
             * You can now access the bag data
             */
            bag.Header
                .Should().NotBeNull();

            bag.Messages
                .Should().NotBeNull();

            bag.Connections
                .Should().NotBeNull();
        }

        [Fact]
        public void Read_bag_from_byte_buffer()
        {
            byte[] buffer = File.ReadAllBytes(ExampleBag.FilePath);
            
            /*
             * Create the bag from a byte array in memory.
             *
             * The BufferBag reads the whole bag in the Create method.
             * If you work with large bags consider using FileBag or StreamBag insterd.
             */
            var bag = BufferBag.Create(buffer);

            bag.Should().NotBeNull();
        }

        [Fact]
        public void Read_bag_from_stream()
        {
            var stream = new MemoryStream(File.ReadAllBytes(ExampleBag.FilePath));
            
            /*
             * Create the bag from any readable stream.
             *
             * The BufferBag reads the whole bag in the Create method.
             * If you work with large bags consider using FileBag or StreamBag insterd.
             */
            var bag = BufferBag.Create(stream);

            bag.Should().NotBeNull();
        }
    }
}