namespace GoogleAPI
{
    public interface GoogleReaderAPI
    {
        bool NeedToGetToken { get; }

        event Message OnDebugMessage;
        event Message OnResponse;
        event Message OnRequest;

        void GetToken();
    }
}