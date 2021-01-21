using System;
using System.Net.Http;
using IgTrading.Models;
using Newtonsoft.Json;

namespace IgTrading
{
    public class IgMarkets
    {
        private EnumIgEnvironment environment;

        public IgMarkets(EnumIgEnvironment environment)
        {
            this.environment = environment;
        }
        public MarketSearchModel Get(SessionModel igSession, string term)
        {
            string action = "/markets?searchTerm=" + term;

            HttpClient httpClient = ClientFactory.Create(igSession, 1);
            IgTradingApiConfig igTradingApiConfig  = new IgTradingApiConfig(environment);

            var response = httpClient.GetAsync(new Uri(igTradingApiConfig.EndPoint() + action)).Result;
            MarketSearchModel result = JsonConvert.DeserializeObject<MarketSearchModel>(response.Content.ReadAsStringAsync().Result);


            return result;
        }
    }
}