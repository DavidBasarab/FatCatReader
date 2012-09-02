using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WinAuthenicationDemo
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public Frame OutputFrame { get; set; }

        private void TestClick(object sender, RoutedEventArgs e)
        {
            var roamingSettings = ApplicationData.Current.RoamingSettings;

            roamingSettings.Values["GoogleCode"] = null;
        }

        private void OnMainPageLoaded(object sender, RoutedEventArgs e)
        {
            GoogleAPI googleAPI = new GoogleAPIHelper();
            googleAPI.OnDebugMessage += DebugPrint;

            var roamingSettings = ApplicationData.Current.RoamingSettings;

            var googleCode = roamingSettings.Values["GoogleCode"];

            if (googleCode == null || string.IsNullOrEmpty(googleCode.ToString()))
            {
                DebugPrint("Could not find GoogleCode.  Going to get it now");

                googleAPI.GetStatusCode();
            }
            else
            {
                DebugPrint("GoogleCode is {0}", googleCode);

                googleAPI.GetToken();
            }
        }

        private void DebugPrint(string message, params object[] args)
        {
            DebugArea.Text += string.Format(message, args) + Environment.NewLine;
        }

        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e"> Event data that describes how this page was reached. The Parameter property is typically used to configure the page. </param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {}
    }
}