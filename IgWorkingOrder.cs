using System;
using System.Net.Http;
using System.Text;
using IgTrading.Models;
using Newtonsoft.Json;

namespace IgTrading
{
    public class IgWorkingOrder
    {

        private EnumIgEnvironment environment;

        public IgWorkingOrder(EnumIgEnvironment environment)
        {
            this.environment = environment;
        }
        public string Post(SessionModel igSession, IgOrder igOrder)
        {
            string action = "/workingorders/otc";
            HttpClient httpClient = ClientFactory.Create(igSession, 2);
            IgTradingApiConfig igTradingApiConfig = new IgTradingApiConfig(environment);

            var content = new StringContent(JsonConvert.SerializeObject(igOrder), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(new Uri(igTradingApiConfig.EndPoint() + action), content).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }      
    }
}