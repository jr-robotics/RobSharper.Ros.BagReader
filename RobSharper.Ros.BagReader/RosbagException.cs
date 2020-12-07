using System;
using System.Runtime.Serialization;

namespace RobSharper.Ros.BagReader
{
    internal class RosbagException : InvalidOperationException
    {
        public RosbagException() : base() {}
        public RosbagException(string message) : base(message) {}
        
        public RosbagException(string message, Exception innerException) : base(message, innerException) {}
        
        public RosbagException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}