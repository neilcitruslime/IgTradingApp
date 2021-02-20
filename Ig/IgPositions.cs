using System;
using System.Net.Http;
using System.Text;
using IgTrading.Ig.Models;
using IgTrading.Models;
using Newtonsoft.Json;

namespace IgTrading.Ig
{
    public class IgPositions
    {
        private EnumIgEnvironment environment;
        private readonly LoginModel login;

        public IgPositions(EnumIgEnvironment environment, LoginModel login)
        {
            this.environment = environment;
            this.login = login;
        }
        public string Get(IgSessionModel igSession)
        {
            string action = "/positions";
            HttpClient httpClient = ClientFactory.Create(igSession, 1);
            IgTradingApiConfig igTradingApiConfig = new IgTradingApiConfig(environment, login);

            var response = httpClient.GetAsync(new Uri(igTradingApiConfig.EndPoint() + action)).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        public string Post(IgSessionModel igSession, IgBuyModel buyOrder)
        {
            string action = "/positions/otc";
            HttpClient httpClient = ClientFactory.Create(igSession, 2);
            IgTradingApiConfig igTradingApiConfig = new IgTradingApiConfig(environment, login);

            var content = new StringContent(JsonConvert.SerializeObject(buyOrder), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(new Uri(igTradingApiConfig.EndPoint() + action), content).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        public void Put(IgSessionModel igSession, PositionPosition position)
        {
            string action = "/positions/otc/";
            HttpClient httpClient = ClientFactory.Create(igSession, 2);
            IgTradingApiConfig igTradingApiConfig = new IgTradingApiConfig(environment, login);

            StringContent content = new StringContent(JsonConvert.SerializeObject(new { limitLevel = position.LimitLevel, stopLevel = position.StopLevel, trailingStop = false }), Encoding.UTF8, "application/json");
            var returnData = httpClient.PutAsync(new Uri(igTradingApiConfig.EndPoint() + action + position.DealId), content).Result;
        }
    }
}