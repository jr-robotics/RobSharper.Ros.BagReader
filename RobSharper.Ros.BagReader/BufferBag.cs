using System;
using System.Collections.Generic;
using System.IO;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public class BufferBag : IBag
    {
        public BagHeader Header { get; }
        public IEnumerable<Connection> Connections { get; }
        public IEnumerable<BagMessage> Messages { get; }

        protected BufferBag(BagHeader header, IEnumerable<Connection> connections, IEnumerable<BagMessage> messages)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Connections = connections ?? throw new ArgumentNullException(nameof(connections));
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        public static BufferBag Create(byte[] buffer)
        {
            return Create(new MemoryStream(buffer));
        }

        public static BufferBag Create(string filePath)
        {
            using (var fs = File.OpenRead(filePath))
            {
                return Create(fs);
            }
        }
        
        public static BufferBag Create(Stream buffer)
        {
            var collector = new CollectorVisitor();
            var bagReader = BagReaderFactory.Create(buffer, collector);
            
            bagReader.ProcessAll();

            return new BufferBag(collector.Header, collector.Connections, collector.Messages);
        }
        
        private class CollectorVisitor : MessageVisitor
        {
            private readonly List<BagMessage> _messages;
            
            public BagHeader Header { get; private set; }
            public IEnumerable<BagMessage> Messages => _messages;

            public CollectorVisitor()
            {
                _messages = new List<BagMessage>();
            }
            
            public override void Visit(BagHeader record)
            {
                Header = record;
            }

            public override void Visit(MessageData message, Connection connection)
            {
                var bagMessage = new BagMessage(message, connection);
                _messages.Add(bagMessage);
            }
        }
    }
}