using System;
using System.Net.Http;
using System.Threading.Tasks;
using IgTrading.Ig.Models;
using IgTrading.Models;
using Newtonsoft.Json;

namespace IgTrading.Ig
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
        public MarketSearchModel Get(IgSessionModel igSession, string term, bool getDetail)
        {
            string action = "/markets?searchTerm=" + term;

            HttpClient httpClient = ClientFactory.Create(igSession, 1);
            IgTradingApiConfig igTradingApiConfig  = new IgTradingApiConfig(environment, login);

            var response = httpClient.GetAsync(new Uri(igTradingApiConfig.EndPoint() + action)).Result;
            MarketSearchModel result = JsonConvert.DeserializeObject<MarketSearchModel>(response.Content.ReadAsStringAsync().Result);

            if (result.Markets!=null && getDetail)
            {
                Parallel.ForEach(result.Markets , (market) =>
                                {
                                    market.EpicModel = GetEpic(igSession, market.Epic);
                                });
            }
                        
            return result;
        }

        public EpicModel GetEpic(IgSessionModel igSession, string epic)
        {
            string action = $"/markets/{epic}";

            HttpClient httpClient = ClientFactory.Create(igSession, 1);
            IgTradingApiConfig igTradingApiConfig  = new IgTradingApiConfig(environment, login);

            var response = httpClient.GetAsync(new Uri(igTradingApiConfig.EndPoint() + action)).Result;
            EpicModel result = JsonConvert.DeserializeObject<EpicModel>(response.Content.ReadAsStringAsync().Result);

            return result;
        }
    }
}