using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace HaveIBeenPwnedFromScratch
{
    namespace Data
    {
        [NPoco.TableName("Breach")]
        [NPoco.PrimaryKey("ID")]
        class Breach
        {
            public Guid ID { get; set; }
            public string Domain { get; set; }
            public DateTime BreachDate { get; set; }
            public string Title { get; set; }
            public bool IsVerified { get; set; }
            public DateTime AddedDate { get; set; }
            public string LogoPath { get; set; }

            private static NPoco.Database db = new NPoco.Database("Server=cmogreport;Integrated Security=SSPI;Database=HaveIBeenPwned", NPoco.DatabaseType.SqlServer2008);
            //----------------------------------------------------------------------------------------------------------------
            public override string ToString()
            {
                return $"ID: {ID} \n \t Title: {Title} \n \t Domain: {Domain} \n \t Breach Date: {BreachDate} \n \t" +
                    $" Added Date: {AddedDate} \n\t IsVerified: {IsVerified} \n \t LogoPath: {LogoPath}";
            }
            //----------------------------------------------------------------------------------------------------------------
            public static IEnumerable<Breach> PrintBreachesToConsole()
            {
                List<Breach> item = db.Query<Breach>("select * from Breach").ToList();
                foreach(Breach breach in item)
                {
                    Console.WriteLine(breach);
                }
                return item;
            }
            //----------------------------------------------------------------------------------------------------------------
            public static Breach FromPwned(Pwned obj)
            {
                Breach result = new Breach();
                result.Domain = obj.Domain;
                result.AddedDate = obj.AddedDate;
                result.BreachDate = obj.BreachDate;
                result.LogoPath = obj.LogoPath;
                result.Title = obj.Title;
                result.IsVerified = obj.IsVerified;

                return result;
            }
            //----------------------------------------------------------------------------------------------------------------
            public void SaveBreachToDatabase()
            {
                db.Save<Breach>(this);
            }
            //----------------------------------------------------------------------------------------------------------------
            public static Breach CheckIfBreachExistsInDatabase(string Title)
            {
                //Check database to see if Title passed in exists in the Breach table
                Breach breach = db.FirstOrDefault<Breach>("SELECT * FROM Breach WHERE Title= @0 ", Title);
                return breach;
            }
            //----------------------------------------------------------------------------------------------------------------
        }
        //----------------------------------------------------------------------------------------------------------------
        [NPoco.TableName("Breached_Emails")]
        [NPoco.PrimaryKey("ID", AutoIncrement = false)]
        class Breached_Emails
        {
            public Guid ID { get; set; }
            public string Email { get; set; }

            private static NPoco.Database db = new NPoco.Database("Server=cmogreport;Integrated Security=SSPI;Database=HaveIBeenPwned", NPoco.DatabaseType.SqlServer2008);
            //----------------------------------------------------------------------------------------------------------------
            public override string ToString()
            {
                return $"ID: {ID} \n \t email: {Email}";
            }
            //----------------------------------------------------------------------------------------------------------------
            public static Breached_Emails FromBreach(Breach obj, string email)
            {
                Breached_Emails result = new Breached_Emails();
                result.ID = obj.ID;
                result.Email = email;
                return result;
            }
            //----------------------------------------------------------------------------------------------------------------
            public static IEnumerable<Breached_Emails> PrintBreachedEmailsToConsole()
            {
                List<Breached_Emails> item = db.Query<Breached_Emails>("SELECT * FROM Breached_Emails").ToList();
                foreach (Breached_Emails breach in item)
                {
                    Console.WriteLine(breach);
                }
                return item;
            }
            //----------------------------------------------------------------------------------------------------------------
            public void SaveBreachedEmailToDatabase()
            {
                db.Save<Breached_Emails>(this);
            }
            //----------------------------------------------------------------------------------------------------------------
            public static Breached_Emails CheckIfEmailExistsInDatabase(Breached_Emails breachedEmailsObj)
            {
                Breached_Emails breachedEmails = db.FirstOrDefault<Breached_Emails>("SELECT * FROM Breached_Emails WHERE ID= @0 AND Email= @1"
                    , breachedEmailsObj.ID, breachedEmailsObj.Email);
                return breachedEmails;
            }
            //----------------------------------------------------------------------------------------------------------------
        }
    }

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
        public static List<Pwned> SendRequest(string email)
        {
            //brings in list of emails and sends to RESTendpoint to check for breaches
            List<Pwned> result = new List<Pwned>();
            result.AddRange(RESTendpoint(email));
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
    }
    class Program : Pwned
    {
        static void Main(string[] args)
        {
            var emailList = new List<string>(args) { "mturnbull1001@gmail.com", "mturnbull2@yahoo.com", "turnbullmt05@mansfield.edu", "turnbullmt@cmog.org" };
            foreach (string email in emailList)
            {
                List<Pwned> AllBreaches = SendRequest(email);

                foreach (Pwned pwn in AllBreaches)
                {
                    Data.Breach breachObj = Data.Breach.FromPwned(pwn);
                    Data.Breach breachedEmail = Data.Breach.CheckIfBreachExistsInDatabase(breachObj.Title);
                    
                    if (breachedEmail != null)
                    {
                        Data.Breached_Emails breachedEmailsObj = Data.Breached_Emails.FromBreach(breachedEmail, email);
                        Data.Breached_Emails doesEmailExistInDatabase = Data.Breached_Emails.CheckIfEmailExistsInDatabase(breachedEmailsObj);
                        if (doesEmailExistInDatabase == null)         //Check if the email and breach has already been saved
                        {
                            //save email with breach ID
                            breachedEmailsObj.SaveBreachedEmailToDatabase();
                        }
                    }
                    else
                    {
                        //create new entry of breach
                        breachObj.SaveBreachToDatabase();
                        Data.Breach breachedEmail2 = Data.Breach.CheckIfBreachExistsInDatabase(breachObj.Title);
                        Data.Breached_Emails breachedEmailsObj = Data.Breached_Emails.FromBreach(breachedEmail2, email);
                        //save email with breach ID
                        breachedEmailsObj.SaveBreachedEmailToDatabase();
                    }
                }
            }
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}