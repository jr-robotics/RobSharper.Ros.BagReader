using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RobSharper.Ros.BagReader.Enumerators;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public class BagFile
    {
        public string Path { get; }
        public MessagesCollection Messages { get; }
        
        public ConnectionCollection Connections { get; }

        public BagFile(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Messages = new MessagesCollection(path);
            Connections = new ConnectionCollection(path);
        }
        
        
        public class MessagesCollection : IEnumerable<BagMessage>
        {
            private readonly string _path;

            internal MessagesCollection(string path)
            {
                _path = path;
            }

            public IEnumerator<BagMessage> GetEnumerator()
            {
                var stream = File.OpenRead(_path);
                var enumerator = new BagMessageEnumerator(stream);

                return new DisposingStreamEnumerator<BagMessage>(enumerator, stream);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        
        public class ConnectionCollection : IEnumerable<Connection>
        {
            private readonly string _path;
        
            internal ConnectionCollection(string path)
            {
                _path = path;
            }

            public IEnumerator<Connection> GetEnumerator()
            {
                var stream = File.OpenRead(_path);
                var enumerator = new BagConnectionEnumerator(stream);

                return new DisposingStreamEnumerator<Connection>(enumerator, stream);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}