using System;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public class BagMessage
    {
        public MessageData Message { get; }
        public Connection Connection { get; }
        
        public BagMessage(MessageData message, Connection connection)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }
    }
}