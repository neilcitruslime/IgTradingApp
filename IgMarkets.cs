using System;
using System.Net.Http;
using IgTrading.Models;
using Newtonsoft.Json;

namespace IgTrading
{
    public class IgMarkets
    {
        private readonly LoginModel login;
        private EnumIgEnvironment environment;

        public IgMarkets(EnumIgEnvironment environment, LoginModel login)
        {
            this.login = login;
            this.environment = environment;
        }
        public MarketSearchModel Get(SessionModel igSession, string term)
        {
            string action = "/markets?searchTerm=" + term;

            HttpClient httpClient = ClientFactory.Create(igSession, 1);
            IgTradingApiConfig igTradingApiConfig  = new IgTradingApiConfig(environment, login);

            var response = httpClient.GetAsync(new Uri(igTradingApiConfig.EndPoint() + action)).Result;
            MarketSearchModel result = JsonConvert.DeserializeObject<MarketSearchModel>(response.Content.ReadAsStringAsync().Result);


            return result;
        }

        public void GetEpic(SessionModel igSession, string epic)
        {
            string action = $"/markets/{epic}";

            HttpClient httpClient = ClientFactory.Create(igSession, 1);
            IgTradingApiConfig igTradingApiConfig  = new IgTradingApiConfig(environment, login);

            var response = httpClient.GetAsync(new Uri(igTradingApiConfig.EndPoint() + action)).Result;
            //MarketSearchModel result = JsonConvert.DeserializeObject<MarketSearchModel>();
            string data = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(data.FormatJson());
            //return result;
        }
    }
}