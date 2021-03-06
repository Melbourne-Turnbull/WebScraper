﻿using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace OnlineExample
{
    internal class Contributor
{
    public string Login { get; set; }
    public int Contributions { get; set; }
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
                    var contributorsAsJson = sr.ReadToEnd();
                    var contributors = JsonConvert.DeserializeObject<List<Contributor>>(contributorsAsJson);
                    contributors.ForEach(Console.WriteLine);

                    Console.WriteLine();
                    Console.WriteLine("All Contributors with more than 20 contributions");
                    IEnumerable<Contributor> log = from t in contributors
                                                   where t.Contributions > 20
                                                   select t;
                    foreach (Contributor Contributions in log)
                    {
                        Console.WriteLine(Contributions);
                    }

                    Console.WriteLine();
                    Console.WriteLine("All Contributors with a j in their username");
                    IEnumerable<Contributor> logs = from t in contributors
                                                    where t.Login.Contains("j")
                                                    select t;
                    foreach (Contributor Login in logs)
                    {
                        Console.WriteLine(Login);
                    }

                    Console.WriteLine();
                    Console.WriteLine("All Contributors with a t in their username");
                    IEnumerable<Contributor> result = contributors.FindAll(x => x.Login.Contains("t")).ToList();
                    foreach(Contributor Login in result)
                    {
                        Console.WriteLine(Login);
                    }
                    Console.WriteLine();
                    Console.WriteLine("All Contributors with a r in their username");
                    List<Contributor> list = contributors.FindAll(x => x.Login.Contains("r")).ToList();
                    foreach(Contributor Login in list)
                    {
                        Console.WriteLine(Login);
                    }
                }
            }
            Console.ReadLine();
        }
    }
}