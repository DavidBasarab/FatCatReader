namespace GoogleReaderAPI
{
    public sealed class Configuration
    {
        public Configuration(DataRepository dataRepository)
        {
            DataRepository = dataRepository;
        }

        private DataRepository DataRepository { get; set; }

        public string Username
        {
            get { return DataRepository.GetUsername(); }
            set { DataRepository.SaveUsername(value); }
        }
    }
}