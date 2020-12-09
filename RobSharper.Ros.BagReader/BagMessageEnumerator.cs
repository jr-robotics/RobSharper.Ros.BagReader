using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public class BagMessageEnumerator : IEnumerator<BagMessage>
    {
        private readonly IBagReader _bagReader;
        private readonly Visitor _visitor;

        public BagMessage Current { get; private set; }

        object IEnumerator.Current => Current;
        
        public BagMessageEnumerator(Stream bagStream)
        {
            if (bagStream == null) throw new ArgumentNullException(nameof(bagStream));
            
            _visitor = new Visitor();
            _bagReader = BagReaderFactory.Create(bagStream, _visitor);
        }

        public bool MoveNext()
        {
            while (_bagReader.ProcessNext())
            {
                if (Current != _visitor.CurrentMessage)
                {
                    Current = _visitor.CurrentMessage;
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            _bagReader.Reset();
            Current = null;
        }

        public void Dispose()
        {
            Reset();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        private class Visitor : MessageVisitorBase
        {
            public BagMessage CurrentMessage { get; private set; }

            public override void Visit(MessageData message, Connection connection)
            {
                CurrentMessage = new BagMessage(message, connection);
            }

            public override void Reset()
            {
                base.Reset();

                CurrentMessage = null;
            }
        }
    }
}