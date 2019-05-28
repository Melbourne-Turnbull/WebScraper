using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OnlineExample
{

    internal class Contributor
{
    public string Login { get; set; }
    public short Contributions { get; set; }

    public override string ToString()
    {
        return $"{Login,20}: {Contributions} contributions";
    }
}

    internal class Program
    {
        private static void Main()
        {
            var webRequest = WebRequest.Create("https://api.github.com/repos/twilio/twilio-csharp/contributors") as HttpWebRequest;
            if (webRequest == null)
            {
                return;
            }

            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Nothing";

            using (var s = webRequest.GetResponse().GetResponseStream())
            {
                using (var sr = new StreamReader(s))
                {
                    //test
                    var contributorsAsJson = sr.ReadToEnd();
                    var contributors = JsonConvert.DeserializeObject<List<Contributor>>(contributorsAsJson);
                    contributors.ForEach(Console.WriteLine);
                    //IEnumerable<JToken> pricyProducts = contributors.SelectTokens("$..Login[?(@.Contributions >= 50)].Login");
                }
            }

            Console.ReadLine();
        }
    }
}