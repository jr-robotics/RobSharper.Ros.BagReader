using System.IO;
using FluentAssertions;
using Xunit;

namespace RobSharper.Ros.BagReader.Examples
{
    public class LargeBagExamples
    {
        [Fact]
        public void Read_large_bag_from_file()
        {
            /*
             * Create am IBag from a rosbag file.
             *
             * The FileBag reads data on demand and does not store any values except the BagHeader in memory.
             *
             * FileBag is thread safe. Every property access opens a new file stream
             */
            var bag = FileBag.Create(ExampleBag.FilePath);

            /*
             * You can now access the bag data
             */
            
            /*
             * Accessing to Header the first time opens the file and reads the header
             */
            bag.Header
                .Should().NotBeNull();

            bag.Messages
                .Should().NotBeNull();

            bag.Connections
                .Should().NotBeNull();

            /*
             * Iterating over Messages or Connections opens the file
             * and starts reading.
             * The file is closed when leaving the foreach scope (closing bracket)
             */
            foreach (var message in bag.Messages)
            {
                message
                    .Should().NotBeNull();
            }
        }
        
        [Fact]
        public void Read_large_bag_from_stream()
        {
            using (var fs = File.OpenRead(ExampleBag.FilePath))
            {
                /*
                 * Create am IBag from a rosbag stream.
                 *
                 * The StreamBag reads data on demand and does not store any values except the BagHeader in memory.
                 * Be sure that the stream is open as long as you use the StreamBag!
                 *
                 * StreamBag is thread safe, but blocking. Every property access reads the stream from the beginning.
                 */
                var bag = new StreamBag(fs);

                /*
                 * You can now access the bag data
                 */

                /*
                 * Accessing to Header the first time opens the file and reads the header
                 */
                bag.Header
                    .Should().NotBeNull();

                bag.Messages
                    .Should().NotBeNull();

                bag.Connections
                    .Should().NotBeNull();

                /*
                 * Iterating over Messages or Connections resets the stream to the initial position
                 * and starts reading.
                 * The stream is blocked for other accesses until the foreach scope is left (closing bracket)
                 */
                foreach (var message in bag.Messages)
                {
                    message
                        .Should().NotBeNull();
                }
            }
        }
    }
}