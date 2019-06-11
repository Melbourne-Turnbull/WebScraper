using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace HaveIBeenPwnedButBetter
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

        private static string API_RootPath = "https://haveibeenpwned.com/api/v2";
        private static string API_BreachServicePath = API_RootPath + "/breachedaccount/";


        //----------------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return $"Name: {Name} \n \t Title: {Title} \n \t Domain: {Domain} \n \t Breach Date: {BreachDate} \n \t" +
                $" Added Date: {AddedDate} \n \t Modified Date: {ModifiedDate} \n \t PwnCount: {PwnCount} \n \t" +
                $" Description: {Description} \n \t DataClasses: {DataClasses} \n \t IsVerified: {IsVerified} \n \t" +
                $" IsFabricated: {IsFabricated} \n \t IsSensitive: {IsSensitive} \n \t IsRetired: {IsRetired} \n \t" +
                $" IsSpamList: {IsSpamList} \n \t LogoPath: {LogoPath}";
        }
        //----------------------------------------------------------------------------------------------------------------
        public static IEnumerable<Pwned> APIEndpoint(string email)       //Make independant endpoint
        {
            var requestPath = String.Format("{0}/{1}", Pwned.API_BreachServicePath, email);
            string ResultAsJson = string.Empty;
            List<Pwned> result = new List<Pwned>();

            HttpWebRequest webRequest = WebRequest.Create(requestPath) as HttpWebRequest;

            if (webRequest == null)
            {
                return null;
            }
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "HaveIBeenPwnedAPIExample";

            try
            {
                using (Stream s = webRequest.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(s))
                    {
                        ResultAsJson = sr.ReadToEnd();
                        result.AddRange(DeserializeToObject(ResultAsJson));
                        return result;
                    }
                }
            }
            catch (System.Net.WebException)
            {
            }
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------
        public static List<Pwned> DeserializeToObject(string ResultAsJson)        //Seems fine so far
        {
            if (!String.IsNullOrEmpty(ResultAsJson))
                return JsonConvert.DeserializeObject<List<Pwned>>(ResultAsJson);
            else
                return new List<Pwned>();  //empty list
        }
        //----------------------------------------------------------------------------------------------------------------
        /*public static IEnumerable<Pwned> Argument(List<Pwned> result)        //Not important at the moment
        {
            IEnumerable<Pwned> list = result.FindAll(x => x.IsSpamList.Equals(false));
            foreach (Pwned IsSpamList in list)
            {
                Console.WriteLine(IsSpamList);
            }
            return list;
        }*/
        //----------------------------------------------------------------------------------------------------------------
        public static void PrintToConsole(List<Pwned> result)
        {
            result.ForEach(Console.WriteLine);
        }
        //----------------------------------------------------------------------------------------------------------------
        public static List<Pwned> CheckAllEmails(List<string> emails)      //Seems fine for the moment
        {
            List<Pwned> result = new List<Pwned>();
            foreach (string email in emails)
            {
                IEnumerable<Pwned> breaches = APIEndpoint(email);
                result.AddRange(breaches);
            }
            return result;
        }
    }
    //----------------------------------------------------------------------------------------------------------------
    public class Program : Pwned
    {
        public static void Main(string[] args)
        {
            bool debugging = true;
            //I want this here because this is the easiest place to populate the list from a commandline argument
            var emailsToCheck = new List<string>() { "mturnbull2@yahoo.com", "turnbullmt05@mansfield.edu", "turnbullmt@cmog.org" };
            //var emailsToCheck = new List<string>(args);
            List<Pwned> AllBreaches = CheckAllEmails(emailsToCheck);
            if (debugging)
                PrintToConsole(AllBreaches);
            else;
            //do something else, like write to SQL server

            Console.ReadKey();
        }
    }
}
