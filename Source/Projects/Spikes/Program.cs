using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Spikes
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Clear();

            try
            {
                GoogleReaderSpike.RunSpike(args);
            }
            catch (Exception ex)
            {
                ex.PrintToConsole();
            }
        }

        public class GoogleReaderSpike
        {
            public static void RunSpike(string[] args)
            {
                if (args.Count() != 2)
                {
                    Extensions.WriteLineWithColor(ConsoleColor.Red, "For this spike a username and password is required as command arguments");

                    return;
                }

                new GoogleReaderSpike(args[0], args[1]).Run();
            }

            private GoogleReaderSpike(string username, string password)
            {
                Username = username;
                Password = password;
            }

            private string Username { get; set; }
            private string Password { get; set; }
            private string SID { get; set; }
            private string Token { get; set; }

            private void GetATOM()
            {
                Extensions.WriteLineWithColor(ConsoleColor.Magenta, "Attempting to GetATOM");

                var url = "http://www.google.com/reader/atom/reading-list/";

                var responseText = GetResponseText(url);

                //Extensions.WriteLineWithColor(ConsoleColor.Cyan, responseText);

                var foundText = responseText.Contains("scottgu: RT @shanselman: Excellent and detailed article on the");

                if (foundText) Extensions.WriteLineWithColor(ConsoleColor.Green, "Found TEXT!!!!!!");
                else Extensions.WriteLineWithColor(ConsoleColor.DarkRed, "DID NOT FIND TEXT!!!!!!");

                Console.WriteLine(responseText);
            }

            private string GetResponseText(string url)
            {
                var cookie = new Cookie("SID", SID, "/", ".google.com");
                

                var request = WebRequest.Create(url) as HttpWebRequest;

                request.Method = "GET";
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookie);

                var response = request.GetResponse() as HttpWebResponse;

                var responseText = string.Empty;

                using (var stream = response.GetResponseStream()) using (var reader = new StreamReader(stream)) responseText = reader.ReadToEnd();
                return responseText;
            }

            private void GetSid()
            {
                var requestUrl = string.Format("https://www.google.com/accounts/ClientLogin?service=reader&Email={0}&Passwd={1}", Username, Password);

                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = "GET";

                var response = (HttpWebResponse)request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    var reader = new StreamReader(stream);

                    var responseString = reader.ReadToEnd();

                    Extensions.WriteLineWithColor(ConsoleColor.Green, "Response = {0}", responseString);

                    var indexSid = responseString.IndexOf("SID=") + 4;
                    var indexLSIDE = responseString.IndexOf("LSID=");

                    SID = responseString.Substring(indexSid, indexLSIDE - 5);

                    Extensions.WriteLineWithColor(ConsoleColor.Yellow, "SID = {0}", SID);
                }
            }

            private void GetToken()
            {
                Extensions.WriteLineWithColor(ConsoleColor.Magenta, "Attempting to GetToken");

                var url = "http://www.google.com/reader/api/0/token";

                var responseText = GetResponseText(url);

                Token = responseText;

                Extensions.WriteLineWithColor(ConsoleColor.Yellow, "Token: {0}", Token);
            }

            private void Run()
            {
                GetSid();
                GetToken();
                //GetATOM();
            }
        }
    }
}