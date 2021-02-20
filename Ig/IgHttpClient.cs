
using IgTrading.Ig.Models;
using IgTrading.Models;
using System;
using System.Net.Http;
using System.Threading;

namespace IgTrading.Ig
{
    public class IgHttpClient
    {
        private EnumIgEnvironment environment;
        private readonly LoginModel login;
        private IgTradingApiConfig igTradingApiConfig;
        public IgHttpClient(EnumIgEnvironment environment, LoginModel login)
        {
            this.login = login;
            this.environment = environment;
            igTradingApiConfig = new IgTradingApiConfig(environment, login);
        }

        public string Get(IgSessionModel igSession, string action, int apiVersion)
        {
            using HttpClient httpClient = ClientFactory.Create(igSession, apiVersion);

            HttpResponseMessage response = httpClient.GetAsync(GetUri(action)).Result;
            return RateLimitedReadResponse(response);
        }

        private static string RateLimitedReadResponse(HttpResponseMessage response)
        {
            string result = string.Empty;
            while (string.IsNullOrWhiteSpace(result))
            {
                result = response.Content.ReadAsStringAsync().Result;
                if (result.Contains("error.public-api.exceeded-api-key-allowance"))
                {
                    result = string.Empty;
                    Thread.Sleep(1000 * 60);
                }
            }
            return result;
        }

        private Uri GetUri(string action)
        {
            return new Uri(igTradingApiConfig.EndPoint() + action);
        }

        public string Post(IgSessionModel igSession, string action, int apiVersion, StringContent content)
        {
            using HttpClient httpClient = ClientFactory.Create(igSession, apiVersion);

            HttpResponseMessage response = httpClient.PostAsync(GetUri(action), content).Result;

            return RateLimitedReadResponse(response);
        }
    }
}
