using System.Collections.Generic;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public abstract class MessageVisitorBase : IBagRecordVisitor
    {
        private readonly Dictionary<int, Connection> _connections;

        public MessageVisitorBase()
        {
            _connections = new Dictionary<int, Connection>();
        }

        public IEnumerable<Connection> Connections => _connections.Values;

        public virtual void Visit(BagHeader record)
        {
        }

        public virtual void Visit(Chunk record)
        {
        }

        public virtual void Visit(Connection record)
        {
            _connections[record.ConnectionId] = record;
        }

        public virtual void Visit(MessageData record)
        {
            var connection = _connections[record.ConnectionId];
            Visit(record, connection);
        }

        public abstract void Visit(MessageData record, Connection connection);

        public virtual void Visit(IndexData record)
        {
        }

        public virtual void Visit(ChunkInfo record)
        {
        }
    }
}