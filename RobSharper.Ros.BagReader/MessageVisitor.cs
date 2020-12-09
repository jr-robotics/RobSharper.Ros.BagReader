using System.Collections.Generic;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public abstract class MessageVisitor : RecordVisitor
    {
        private readonly Dictionary<int, Connection> _connections;

        public MessageVisitor()
        {
            _connections = new Dictionary<int, Connection>();
        }

        public IEnumerable<Connection> Connections => _connections.Values;

        public override void Visit(Connection record)
        {
            record.ReadData();
            _connections[record.ConnectionId] = record;
        }

        public override void Visit(MessageData record)
        {
            record.ReadData();
            
            var connection = _connections[record.ConnectionId];
            Visit(record, connection);
        }

        public abstract void Visit(MessageData message, Connection connection);

        public override void Reset()
        {
            _connections.Clear();
        }
    }
}