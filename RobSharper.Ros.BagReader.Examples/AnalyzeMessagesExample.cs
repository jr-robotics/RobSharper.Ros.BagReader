using System.Linq;
using FluentAssertions;
using RobSharper.Ros.MessageEssentials;
using RobSharper.Ros.MessageEssentials.Serialization;
using Xunit;

namespace RobSharper.Ros.BagReader.Examples
{
    public class AnalyzeMessagesExample
    {
        [Fact]
        public void Read_bag_and_analyze_messages_example()
        {
            // Open rosbag
            var bag = BufferBag.Create(ExampleBag.FilePath);

            // Create a RobSharper.Ros.MessageEssentials Serializer
            var serializer = CreateSerializer();

            // Filter messages
            var chatterMessages = bag.Messages
                .Where(message => message.Connection.DataTopic.Equals("/chatter"));

            // Iterate over filtered messages
            foreach (var message in chatterMessages)
            {
                var messageData = message.Message.Data;
                var stringMessage = serializer.Deserialize<StringMessage>(messageData);

                stringMessage.Should().NotBeNull();
                stringMessage.Data.Should().NotBeNull();
            }
        }

        private RosMessageSerializer CreateSerializer()
        {
            /*
             * This method uses a TypeRegistry and RosMessageSerializer
             * from RobSharper.Ros.MessageEssentials project.
             *
             * See project manual for further documentation.
             */
            
            // The TypeRegistry stores all known ROS types and their mappings to CLR types
            var typeRegistry = new MessageTypeRegistry();
            
            // Create a serializer for the registered types
            var serializer = new RosMessageSerializer(typeRegistry);
            return serializer;
        }

        [RosMessage(RosType = "std_msgs/String")]
        public class StringMessage
        {
            [RosMessageField(RosType = "string", RosIdentifier = "data", Index = 1)]
            public string Data { get; set; }
        }
    }
}