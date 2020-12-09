using System.Collections.Generic;
using RobSharper.Ros.BagReader.Records;

namespace RobSharper.Ros.BagReader
{
    public interface IBag
    {
        BagHeader Header { get; }
        
        IEnumerable<Connection> Connections { get; }
        
        IEnumerable<BagMessage> Messages { get; }
    }
}