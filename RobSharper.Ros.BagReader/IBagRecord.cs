namespace RobSharper.Ros.BagReader
{
    public interface IBagRecord
    {
        void Accept(IBagRecordVisitor visitor);
    }
}