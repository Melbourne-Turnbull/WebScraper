﻿using System;
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

        private static string API_RootPath = "https://haveibeenpwned.com/api/v2";
        private static string API_BreachServicePath = API_RootPath+"/breachedaccount/";


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
        public static HttpWebRequest RequestWebpage(string email, HttpWebRequest webRequest)
        {
            //I like this model, I would chase down the reason for the buggy response behavior
            if (webRequest == null)
            {
                return null;
            }
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "HaveIBeenPwnedAPIExample";
            return webRequest;
        }
        //----------------------------------------------------------------------------------------------------------------
        public static List<Pwned> DeserializeToObject(string jsonString)
        {
            if (!String.IsNullOrEmpty(jsonString))
                return JsonConvert.DeserializeObject<List<Pwned>>(jsonString);
            else
                return new List<Pwned>();  //empty list
        }
        //----------------------------------------------------------------------------------------------------------------
        public static IEnumerable<Pwned> Argument(List<Pwned> result)
        {
            IEnumerable<Pwned> list = result.FindAll(x => x.IsSpamList.Equals(false));
            foreach (Pwned IsSpamList in list)
            {
                Console.WriteLine(IsSpamList);
            }
            return list;
        }
        //----------------------------------------------------------------------------------------------------------------
        public static void PrintToConsole(List<Pwned> result)
        {
            result.ForEach(Console.WriteLine);
        }
        //----------------------------------------------------------------------------------------------------------------
        public static List<Pwned> CheckAllEmails(List<string> emails)
        {
            //confused about why i'm receiving an error message here...  trying a few things
            List<Pwned> result = new List<Pwned>();
            foreach (string email in emails)
            {
                IEnumerable<Pwned> breaches = CheckSpecificEmail(email);
                result.AddRange(breaches);
            }
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------
        public static IEnumerable<Pwned> CheckSpecificEmail(string email)
        {
            //Console.WriteLine("Please enter email address to check");
            //var email = Console.ReadLine();
            var requestPath = String.Format("{0}/{1}", Pwned.API_BreachServicePath, email);
            string ResultAsJson = string.Empty;
            List<Pwned> result = new List<Pwned>();

            HttpWebRequest webRequest = WebRequest.Create(requestPath) as HttpWebRequest;

            //bringing this back into the current method for now, will leave deserialization as separate method
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
                    }
                }
            }
            catch (System.Net.WebException)
            {
                //nothing to do here, ResultAsJson remains empty
                //return null;
            }

            //RequestWebpage(email, webRequest); //my concern with this is that it doesn't do the whole web request, so the name is misleading
            //DeserializeToObject(email, webRequest); //my concern with this is that this function *does* do part of the web request, so that element of the process is split among two functions

            result.AddRange(DeserializeToObject(ResultAsJson));
                       
            return result;
        }
    }
    //----------------------------------------------------------------------------------------------------------------
    public class Program : Pwned
    {
        public static void Main(string[] args)
        {
            bool debugging = true ;
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
