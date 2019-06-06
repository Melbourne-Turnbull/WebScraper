using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Parameters
{
    public class Contributor
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Domain { get; set; }
        public DateTime BreachDate { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int PwnCount { get; set; }
        public string Description { get; set; }
        public string[] DataClasses { get; set; }
        public Boolean IsVerified { get; set; }
        public Boolean IsFabricated { get; set; }
        public Boolean IsSensitive { get; set; }
        public Boolean IsRetired { get; set; }
        public Boolean IsSpamList { get; set; }
        public string LogoPath { get; set; }

        public override string ToString()
        {
            return $"Name: {Name} \n \t Title: {Title} \n \t Domain: {Domain} \n \t Breach Date: {BreachDate} \n \t" +
                $" Added Date: {AddedDate} \n \t Modified Date: {ModifiedDate} \n \t PwnCount: {PwnCount} \n \t" +
                $" Description: {Description} \n \t DataClasses: {DataClasses} \n \t IsVerified: {IsVerified} \n \t" +
                $" IsFabricated: {IsFabricated} \n \t IsSensitive: {IsSensitive} \n \t IsRetired: {IsRetired} \n \t" +
                $" IsSpamList: {IsSpamList} \n \t LogoPath: {LogoPath}";
        }
        public void GetWebsite()
        {
            string[] email = new string[3] { "mturnbull2@yahoo.com", "turnbullmt05@mansfield.edu", "turnbullmt@cmog.org" };
            for (int i = 0; i < email.Length; i++)
            {
                //Console.WriteLine("Please enter email address");
                //var email = Console.ReadLine();
                Console.WriteLine(email[i]);
                var webRequest = WebRequest.Create("https://haveibeenpwned.com/api/v2/breachedaccount/" + email[i]) as HttpWebRequest;
                if (webRequest == null)
                {
                    return;
                }
                webRequest.Method = "GET";
                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Parameters";
                try
                {
                    using (Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (var sr = new StreamReader(s))
                        {
                            var contributorsAsJson = sr.ReadToEnd();
                            var contributors = JsonConvert.DeserializeObject<List<Contributor>>(contributorsAsJson);
                            contributors.ForEach(Console.WriteLine);
                            //Console.WriteLine(\n \n \n);
                            //Console.WriteLine("IsSpamList equals True");
                            //IEnumerable<Contributor> list = contributors.FindAll(x => x.IsSpamList.Equals(true));
                            //foreach (Contributor Login in list)
                            //{
                            //    Console.WriteLine(Login);
                            //}
                        }
                    }
                }
                catch (System.Net.WebException)
                {
                    Console.WriteLine(email[i] + " has not been breached!");
                }
            }
        }
    }
    public class Program
    {
        private static void Main()
        {
            Contributor obj = new Contributor();
            obj.GetWebsite();
            Console.ReadLine();
        }
       
    }
}