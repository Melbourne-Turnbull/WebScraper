using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace HaveIBeenPwnedApi
{
    public class Pwned
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
        public static HttpWebRequest RequestWebpage(string email, HttpWebRequest webRequest)
        {
            if (webRequest == null)
            {
                return null;
            }
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "HaveIBeenPwnedAPIExample";
            return webRequest;
        }
        public static List<Pwned> DeserializeToObject(string email, HttpWebRequest webRequest)
        {
            try
            {
                using (Stream s = webRequest.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(s))
                    {
                        var ResultAsJson = sr.ReadToEnd();
                        List<Pwned> result = JsonConvert.DeserializeObject<List<Pwned>>(ResultAsJson);
                        //Argument(result);
                        PrintToConsole(result);
                        return result;
                    }
                }
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine(email + " has not been breached!");
                return null ;
            }
        }
        public static IEnumerable<Pwned> Argument(List<Pwned> result)
        {
            IEnumerable<Pwned> list = result.FindAll(x => x.IsSpamList.Equals(false));
            foreach (Pwned IsSpamList in list)
            {
                Console.WriteLine(IsSpamList);
            }
            return list;
        }
        public static IEnumerable<Pwned> PrintToConsole(List<Pwned> result)
        {
            result.ForEach(Console.WriteLine);
            return null;
        }
        public static IEnumerable<Pwned> CheckAllEmails()
        {
            var emails = new List<string>() { "mturnbull2@yahoo.com", "turnbullmt05@mansfield.edu", "turnbullmt@cmog.org" };
            foreach (string email in emails)
            {
                HttpWebRequest webRequest = WebRequest.Create("https://haveibeenpwned.com/api/v2/breachedaccount/" + email) as HttpWebRequest;
                Console.WriteLine("\n" + email);
                RequestWebpage(email, webRequest);
                DeserializeToObject(email, webRequest);
                Console.WriteLine("done");
            }
            return null;
        }
        public static IEnumerable<Pwned> CheckSpecificEmail()
        {
            Console.WriteLine("Please enter email address to check");
            var email = Console.ReadLine();
            HttpWebRequest webRequest = WebRequest.Create("https://haveibeenpwned.com/api/v2/breachedaccount/" + email) as HttpWebRequest;
            Console.WriteLine("\n" + email);
            RequestWebpage(email, webRequest);
            DeserializeToObject(email, webRequest);
            Console.WriteLine("done");
            return null;
        }
    }
    public class Program: Pwned
    {
        public static void Main()
        {
            CheckAllEmails();
            //CheckSpecificEmail();
            Console.ReadKey();
        }
    }
}
