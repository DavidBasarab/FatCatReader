using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Windows.Security.Authentication.Web;
using Windows.Storage;

namespace WinAuthenicationDemo
{
    public delegate void DebugMessage(string message, params object[] args);

    public interface GoogleAPI
    {
        event DebugMessage OnDebugMessage;

        void GetStatusCode();

        void GetToken();
    }

    public class GoogleAPIHelper : GoogleAPI
    {
        public HttpClient HttpClient { get; set; }

        public ApplicationDataContainer RoamingSettings
        {
            get { return ApplicationData.Current.RoamingSettings; }
        }

        public event DebugMessage OnDebugMessage;

        public async void GetStatusCode()
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

        public async void GetToken()
        {
            GetResponseText("http://www.google.com/reader/api/0/token");
        }

        private void DebugPrint(string message, params object[] args)
        {
            if (OnDebugMessage != null) OnDebugMessage(message, args);
        }

        private async void GetResponseText(string url)
        {
            HttpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            //request.Headers.Add("Authorization", string.Format("GoogleLogin auth={0}", RetrieveGoogleCode()));
            request.Headers.Add("Authorization", string.Format("GoogleLogin auth= \"{0}\"", RetrieveGoogleCode()));

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            DebugPrint("StatusCode: {0} - {1}", response.StatusCode, response.ReasonPhrase);

            var responseBody = new StringBuilder();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var streamReader = new StreamReader(responseStream);

                responseBody.Append(streamReader.ReadToEnd());
            }

            DebugPrint("Response: {0}", responseBody);
        }

        private string RetrieveGoogleCode()
        {
            return RoamingSettings.Values["GoogleCode"].ToString().Replace("Success code=", string.Empty);
        }

        private void StoreGoogleCode(string responseData)
        {
            RoamingSettings.Values["GoogleCode"] = responseData;
        }
    }
}