﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebScraperAPIExample
{
    class Program
    {
        public class Reddit
        {
            public string id { get; set; }
            public string name { get; set; }
            public string kind { get; set; }
            public object data { get; set; }
        }
        static void Main(string[] args)
        {
            Task t = new Task(DownloadPageAsync);
            t.Start();
            Console.WriteLine("Downloading page...");
            Console.ReadLine();
        }

        static async void DownloadPageAsync()
        {
            string page = "http://www.reddit.com/.json";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                Reddit json = JsonConvert.DeserializeObject<Reddit>(result);
                Console.WriteLine(json.kind);
                Console.WriteLine(json.data);
                Console.WriteLine(json.id);
                Console.WriteLine(json.name);
            }
        }
    }
}
