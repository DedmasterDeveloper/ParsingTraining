using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BankiRuJsonParsing
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var resultat = GetData(url: "https://www.banki.ru/products/currency/usd/");
                if(resultat != null)
                {
                    Console.Title = "Banki.ru";
                    Console.WriteLine(string.Join("\t|\t", resultat.candles.columns));
                    foreach(var row in resultat.candles.data)
                    {
                        Console.WriteLine(string.Join("\t|\t", row));
                    }
                }

                Console.ReadKey();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private static BankiRuRespons GetData(string url)
        {
            try
            {
                using(HttpClientHandler hdl = new HttpClientHandler { AllowAutoRedirect = false, AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.None, 
                    CookieContainer = new CookieContainer()})
                {
                    //using(HttpClient clnt = new HttpClient(hdl))
                    //{
                    //    clnt.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                    //    clnt.DefaultRequestHeaders.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
                    //    clnt.DefaultRequestHeaders.Add("Accept-Language", "ru-RU");
                    //    clnt.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                    //    clnt.DefaultRequestHeaders.Add("Host", "www.banki.ru");
                    //    //clnt.DefaultRequestHeaders.Add("", "");

                    //    using(var resp = clnt.GetAsync(url))
                    //    {
                            
                    //    }
                    //}

                    using(HttpClient clnt = new HttpClient(hdl))
                    {
                        clnt.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                        clnt.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                        clnt.DefaultRequestHeaders.Add("Accept-Language", "ru-RU");
                        clnt.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                        clnt.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                        clnt.DefaultRequestHeaders.Add("Referer", url);

                        DateTime date = DateTime.Today;
                        string Date = date.ToString("yyyy-MM-dd");

                        using (var resp = clnt.GetAsync($"https://www.banki.ru/moex/iss/engines/currency/markets/selt/securities/USD000UTSTOM/candles.json?from={Date}&interval=1&start=0").Result)
                        {
                            var json = resp.Content.ReadAsStringAsync().Result;
                            if(!string.IsNullOrEmpty(json))
                            {
                                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BankiRuRespons>(json);
                                return result;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
