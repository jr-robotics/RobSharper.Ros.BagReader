using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RobSharper.Ros.BagReader.Enumerators;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public class FileBag : IBag
    {
        private BagHeader _header;
        public string Path { get; }

        public BagHeader Header
        {
            get
            {
                if (_header == null)
                {
                    _header = ReadHeader();
                }

                return _header;
            }
        }

        private BagHeader ReadHeader()
        {
            var visitor = new HeaderVisitor();
            
            using (var fs = File.OpenRead(Path))
            {
                var reader = BagReaderFactory.Create(fs, visitor);

                do
                {
                    reader.ProcessNext();
                    
                    if (visitor.Header != null)
                        break;
                    
                } while (reader.HasNext());
            }

            return visitor.Header;
        }

        public IEnumerable<BagMessage> Messages { get; }
        public IEnumerable<Connection> Connections { get; }

        public FileBag(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Messages = new MessageCollection(path);
            Connections = new ConnectionCollection(path);
        }

        public static FileBag Create(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Rosbag file {filePath} not found");

            return new FileBag(filePath);
        }

        private abstract class Collection<T> : IEnumerable<T>
        {
            private readonly string _path;

            protected Collection(string path)
            {
                _path = path;
            }

            public IEnumerator<T> GetEnumerator()
            {
                var stream = File.OpenRead(_path);
                var enumerator = GetEnumerator(stream);

                return new DisposingStreamEnumerator<T>(enumerator, stream);
            }

            protected abstract IEnumerator<T> GetEnumerator(Stream stream);
            
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        
        private class MessageCollection : Collection<BagMessage>
        {
            internal MessageCollection(string path) : base(path)
            {
            }

            protected override IEnumerator<BagMessage> GetEnumerator(Stream stream)
            {
                return new BagMessageEnumerator(stream);
            }
        }
        
        private class ConnectionCollection : Collection<Connection>
        {
            internal ConnectionCollection(string path) : base(path)
            {
            }

            protected override IEnumerator<Connection> GetEnumerator(Stream stream)
            {
                return new BagConnectionEnumerator(stream);
            }
        }
    }
}