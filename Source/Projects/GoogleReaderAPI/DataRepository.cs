namespace GoogleReaderAPI
{
    public interface DataRepository
    {
        string GetUsername();

        void SaveUsername(string username);
    }
}