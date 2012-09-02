using System;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WinAuthenicationDemo
{
    public sealed partial class ProcessLogin : Page
    {
        public ProcessLogin()
        {
            InitializeComponent();
        }

        private MainPage RootPage { get; set; }

        public async void OnLaunchClick(object sender, RoutedEventArgs e)
        {
            DebugPrint("OnLaunchClick started ====> ");

            const string googleClientID = "77504385925.apps.googleusercontent.com";
            var googleURL = "https://accounts.google.com/o/oauth2/auth?client_id=" + Uri.EscapeDataString(googleClientID) + "&redirect_uri=" +
                            Uri.EscapeDataString("urn:ietf:wg:oauth:2.0:oob") + "&response_type=code&scope=" + Uri.EscapeDataString("http://picasaweb.google.com/data");

            var startUri = new Uri(googleURL);
            var endUri = new Uri("https://accounts.google.com/o/oauth2/approval?");

            DebugPrint("Navigating to: {0}", googleURL);

            var webAuthResults = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.UseTitle, startUri, endUri);

            if (webAuthResults.ResponseStatus == WebAuthenticationStatus.Success)
            {
                DebugPrint("Authenicaiton successfully.");
                DebugPrint("Token is {0}", webAuthResults.ResponseData);

                StoreGoogleCode(webAuthResults.ResponseData);
            }
            else if (webAuthResults.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                DebugPrint("Authenicaiton has errored.");
                DebugPrint("Error information {0}", webAuthResults.ResponseErrorDetail);
            }
            else
            {
                DebugPrint("Unknown has errored.");
                DebugPrint("Error information {0}", webAuthResults.ResponseStatus);
            }
        }

        private void StoreGoogleCode(string responseData)
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            roamingSettings.Values["GoogleCode"] = responseData;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RootPage = e.Parameter as MainPage;

            base.OnNavigatedTo(e);
        }

        private void DebugPrint(string message, params object[] args)
        {
            DebugArea.Text += string.Format(message, args) + Environment.NewLine;
        }
    }
}