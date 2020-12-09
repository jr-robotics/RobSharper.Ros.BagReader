using System;

namespace RobSharper.Ros.BagReader
{
    public interface IBagRecord : IDisposable
    {
        void Accept(IBagRecordVisitor visitor);
    }
}