using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace EbayScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            { GetHtmlAsync(); Console.ReadLine();}

            Static void GetHtmlAsync()
            {
                var url = @"Http://www.ebay.com/sch/i.html?_nkw=Xbox+One&_in_kw=1&_ex_kw=&_sacat=0&_udlo=&_udhi=&_ftrt=901&_ftrv=1&_sabdlo=&_sabdhi=&_samilow=&_samihi=&_sadis=15&_stpos=14830&_sargn=-1%26saslc%3D1&_salic=1&_sop=12&_dmd=1&_ipg=50&_fosrp=1";
                var httpclient = new HttpClient();
                var html = httpclient.GetStringAsync(url);
                Console.WriteLine(Result.html);
            }
        }
    }
}
