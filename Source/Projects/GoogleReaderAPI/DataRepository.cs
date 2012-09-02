namespace GoogleReaderAPI
{
    public interface DataRepository
    {
        string GetUsername();

        void SaveUsername(string username);

        string GetPassword();

        void SavePassword(string password);
    }
}