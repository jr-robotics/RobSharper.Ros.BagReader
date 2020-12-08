namespace RobSharper.Ros.BagReader
{
    public interface IBagReader
    {
        bool HasNext();
        bool ProcessNext();
        void ProcessAll();
    }
}