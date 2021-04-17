using IgTrading.Ig.Models;
using IgTrading.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace IgTrading.Ig
{
    public class IgWorkingOrder
    {
        private IgHttpClient igHttpClient = new IgHttpClient();
        public string Post(IgSessionModel igSession, IgOrderModel igOrder)
        {
            string action = "/workingorders/otc";
            HttpClient httpClient = ClientFactory.Create(igSession, 2);

            string json = JsonConvert.SerializeObject(igOrder);
            Console.WriteLine(json.FormatJson());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            string result = igHttpClient.Post(igSession, action, 2, content);

            return result;
        }


    }
}