using System;
using System.Net.Http;
using IgTrading.Ig.Models;
using IgTrading.Models;

namespace IgTrading.Ig
{
    public class IgConfirms
    {
        private EnumIgEnvironment environment;
        private readonly LoginModel login;

        public IgConfirms(EnumIgEnvironment environment, LoginModel login)
        {
            this.environment = environment;
            this.login = login;
        }
        
        public string GetConfirms(IgSessionModel igSession, string dealReference)
        {
            string action = $"/confirms/{dealReference}";
            HttpClient httpClient = ClientFactory.Create(igSession, 1);
            IgTradingApiConfig igTradingApiConfig = new IgTradingApiConfig(environment, login);

            var response = httpClient.GetAsync(new Uri(igTradingApiConfig.EndPoint() + action)).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

    }
}