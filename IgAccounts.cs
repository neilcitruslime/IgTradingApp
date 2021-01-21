using System;
using System.Net.Http;
using IgTrading.Models;
using Newtonsoft.Json;


namespace IgTrading
{
    public class IgAccounts
    {
        private EnumIgEnvironment environment;

        public IgAccounts(EnumIgEnvironment environment)
        {
            this.environment = environment;
        }

        public AccountModels Get(SessionModel igSession)
        {
            string action = "/accounts";
            HttpClient httpClient = ClientFactory.Create(igSession, 1);
            IgTradingApiConfig igTradingApiConfig = new IgTradingApiConfig(environment);

            var response = httpClient.GetAsync(new Uri(igTradingApiConfig.EndPoint() + action)).Result;        

            return JsonConvert.DeserializeObject<AccountModels>(response.Content.ReadAsStringAsync().Result);
        }

    }
}