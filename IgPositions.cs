using System;
using System.Net.Http;
using System.Text;
using IgTrading.Models;
using Newtonsoft.Json;

namespace IgTrading
{
    public class IgPositions
    {
        private EnumIgEnvironment environment;

        public IgPositions(EnumIgEnvironment environment)
        {
            this.environment = environment;
        }
        public string Get(SessionModel igSession)
        {
            string action = "/positions";
            HttpClient httpClient = ClientFactory.Create(igSession, 1);
            IgTradingApiConfig igTradingApiConfig = new IgTradingApiConfig(environment);

            var response = httpClient.GetAsync(new Uri(igTradingApiConfig.EndPoint() + action)).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        public string Post(SessionModel igSession, IgBuy buyOrder)
        {
            string action = "/positions/otc";
            HttpClient httpClient = ClientFactory.Create(igSession, 2);
            IgTradingApiConfig igTradingApiConfig = new IgTradingApiConfig(environment);

            var content = new StringContent(JsonConvert.SerializeObject(buyOrder), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(new Uri(igTradingApiConfig.EndPoint() + action), content).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        public void Put(SessionModel igSession, PositionPosition position)
        {
            string action = "/positions/otc/";
            HttpClient httpClient = ClientFactory.Create(igSession, 2);
            IgTradingApiConfig igTradingApiConfig = new IgTradingApiConfig(environment);

            StringContent content = new StringContent(JsonConvert.SerializeObject(new { limitLevel = position.LimitLevel, stopLevel = position.StopLevel, trailingStop = false }), Encoding.UTF8, "application/json");
            var returnData = httpClient.PutAsync(new Uri(igTradingApiConfig.EndPoint() + action + position.DealId), content).Result;
        }
    }
}