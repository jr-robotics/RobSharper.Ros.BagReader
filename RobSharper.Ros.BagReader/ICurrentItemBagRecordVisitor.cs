namespace RobSharper.Ros.BagReader
{
    public interface ICurrentItemBagRecordVisitor<out T> : IBagRecordVisitor
    {
        T Current { get; }
    }
}