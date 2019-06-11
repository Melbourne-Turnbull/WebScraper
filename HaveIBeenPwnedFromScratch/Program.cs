using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace HaveIBeenPwnedFromScratch
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

        //Path to the API
        private static string API_RootPath = "https://haveibeenpwned.com/api/v2";
        private static string API_BreachServicePath = API_RootPath + "/breachedaccount/";
        //----------------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            //format for the returned value
            return $"Name: {Name} \n \t Title: {Title} \n \t Domain: {Domain} \n \t Breach Date: {BreachDate} \n \t" +
                $" Added Date: {AddedDate} \n \t Modified Date: {ModifiedDate} \n \t PwnCount: {PwnCount} \n \t" +
                $" Description: {Description} \n \t DataClasses: {DataClasses} \n \t IsVerified: {IsVerified} \n \t" +
                $" IsFabricated: {IsFabricated} \n \t IsSensitive: {IsSensitive} \n \t IsRetired: {IsRetired} \n \t" +
                $" IsSpamList: {IsSpamList} \n \t LogoPath: {LogoPath}";
        }
        //----------------------------------------------------------------------------------------------------------------
        public static IEnumerable<Pwned> RESTendpoint(string email)
        {
            //finishes the path with the email thats being checked
            var requestPath = String.Format("{0}/{1}", Pwned.API_BreachServicePath, email);
            string ResultAsJson = string.Empty; //place holder
            List<Pwned> result = new List<Pwned>();

            //sending the URL with the request
            HttpWebRequest webRequest = WebRequest.Create(requestPath) as HttpWebRequest;

            if (webRequest == null)
            {
                return null;
            }
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "HaveIBeenPwnedAPIExample";

            //gets response and reads it before sending it to get deserialized
            try
            {
                using (Stream s = webRequest.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(s))
                    {
                        ResultAsJson = sr.ReadToEnd();
                        result.AddRange(Convert(ResultAsJson));
                        return result;
                    }
                }
            }
            //if the URL is invalid/ there was no breaches it returns nothing
            catch (System.Net.WebException)
            {
            }
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------
        public static List<Pwned> SendRequest(List<string> emailList)
        {
            //brings in list of emails and sends to RESTendpoint to check for breaches
            List<Pwned> result = new List<Pwned>();
            foreach (string email in emailList)
            {
                result.AddRange(RESTendpoint(email));
            }
            //Returns the breaches deserialized
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------
        public static List<Pwned> Convert(string ResultAsJson)
        {
            //convert from string to generic list (deserialize method)
            if (!String.IsNullOrEmpty(ResultAsJson))
            {
                return JsonConvert.DeserializeObject<List<Pwned>>(ResultAsJson);
            }
            else
            {
                return new List<Pwned>();  //empty list
            }
        }
        //----------------------------------------------------------------------------------------------------------------
        public static void NewEndpoint(List<Pwned> result, bool SendToDatabase)
        {
            //Boolean in main to check if its being sent to database or console
            if (SendToDatabase)
            {
                //send to MsSql database
            }
            else
            {
                //print to console 
                result.ForEach(Console.WriteLine);
            }
        }
        //----------------------------------------------------------------------------------------------------------------
    }
    class Program : Pwned
    {
        static void Main(string[] args)
        {
            bool SendToDatabase = false;
            var emailList = new List<string>(args) { "mturnbull2@yahoo.com", "turnbullmt05@mansfield.edu", "turnbullmt@cmog.org" };
            List<Pwned> AllBreaches = SendRequest(emailList);
            NewEndpoint(AllBreaches, SendToDatabase);
            Console.ReadKey();
        }
    }
}
