using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WinRTJSON;
using Windows.Security.Authentication.Web;
using Windows.Storage;

namespace GoogleAPI
{
    public delegate void Message(string message, params object[] args);

    public sealed class GoogleAPIHelper : GoogleReaderAPI
    {
        private HttpClient HttpClient { get; set; }

        public ApplicationDataContainer RoamingSettings
        {
            get { return ApplicationData.Current.RoamingSettings; }
        }

        public event Message OnDebugMessage;
        public event Message OnResponse;
        public event Message OnRequest;

        public async void GetToken()
        {
            await GetStatusCode();

            DebugPrint("Geting Token");

            HttpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.google.com/o/oauth2/token");

            var paramters = new StringBuilder();

            paramters.AppendFormat("code={0}&", RetrieveGoogleCode());
            paramters.AppendFormat("client_id={0}&", "77504385925.apps.googleusercontent.com");
            paramters.AppendFormat("client_secret={0}&", "StuSEv8ceP-EQi1WvWLXc6I8");
            paramters.AppendFormat("redirect_uri={0}&", "urn:ietf:wg:oauth:2.0:oob");
            paramters.AppendFormat("grant_type={0}", "authorization_code");

            var requestContent = new StringContent(paramters.ToString());

            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            request.Content = requestContent;

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            DebugPrint("StatusCode: {0} - {1}", response.StatusCode, response.ReasonPhrase);

            var responseBody = new StringBuilder();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var streamReader = new StreamReader(responseStream);

                responseBody.Append(streamReader.ReadToEnd());
            }

            DebugPrint(responseBody.ToString());

            var jsonResults = JSON.JsonDecode(responseBody.ToString()) as IDictionary<string, object>;

            if (jsonResults != null)
            {
                StoreSetting("AccessToken", jsonResults["access_token"].ToString());
                StoreSetting("RefreshToken", jsonResults["refresh_token"].ToString());
            }
        }

        public bool NeedToGetToken
        {
            get { return RoamingSettings == null || KeyValueFound("AccessToken") || KeyValueFound("RefreshToken"); }
        }

        private void DebugPrint(string message, params object[] args)
        {
            if (OnDebugMessage != null) OnDebugMessage(message, args);
        }

        private async Task<int> GetStatusCode()
        {
            DebugPrint("GetStatusCode started ====> ");

            const string googleClientID = "77504385925.apps.googleusercontent.com";
            var googleURL = "https://accounts.google.com/o/oauth2/auth?client_id=" + Uri.EscapeDataString(googleClientID) + "&redirect_uri=" +
                            Uri.EscapeDataString("urn:ietf:wg:oauth:2.0:oob") + "&response_type=code&scope=" + Uri.EscapeDataString("http://picasaweb.google.com/data") + "&access_type=offline";

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

            return 1;
        }

        private bool KeyValueFound(string key)
        {
            return RoamingSettings.Values[key] == null || string.IsNullOrEmpty(RoamingSettings.Values[key].ToString());
        }

        private string RetrieveGoogleCode()
        {
            return RoamingSettings.Values["GoogleCode"].ToString().Replace("Success code=", string.Empty);
        }

        private string RetrieveToken()
        {
            return RoamingSettings.Values["Token"].ToString();
        }

        private void StoreGoogleCode(string googleCode)
        {
            RoamingSettings.Values["GoogleCode"] = googleCode;
        }

        private void StoreSetting(string key, string token)
        {
            RoamingSettings.Values[key] = token;
        }

        private void StoreToken(string token) {}
    }
}