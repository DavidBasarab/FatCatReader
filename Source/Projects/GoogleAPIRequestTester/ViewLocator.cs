using GoogleAPIRequestTester.ViewModels;

namespace GoogleAPIRequestTester
{
    public class ViewLocator
    {
        public static MainViewModel MainViewModel { get; private set; }

        static ViewLocator()
        {
            try
            {
                MainViewModel = new MainViewModel();
            }
            catch {}
        }
    }
}