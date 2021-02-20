﻿namespace IgTrading.Ig
{
    using IgTrading.Ig.Models;
    using IgTrading.Models;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    public class IgTradingApiConfig
    {
        private static string ApiEndPoint = "gateway/deal";
        private static string Demo = "https://demo-api.ig.com/";
        private static string Live = "https://api.ig.com/";

        EnumIgEnvironment environment;
        private readonly LoginModel login;

        public IgTradingApiConfig(EnumIgEnvironment environment, LoginModel login)
        {
            this.environment = environment;
            this.login = login;
        }

        public string EndPoint()
        {
            if (environment == EnumIgEnvironment.Live)
            {
                return $"{Live}{ApiEndPoint}";
            }
            else
            {
                return $"{Demo}{ApiEndPoint}";
            }
        }

        public IgSessionModel Session()
        {
            string action = "/session";

            HttpClient httpClient = ClientFactory.Create();

            var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

            string Url = EndPoint() + action;
            var responseMessage = httpClient.PostAsync(new Uri(Url), content).Result;
            IgSessionModel igSession = GetSession(responseMessage);

            return igSession;
        }

        private static IgSessionModel GetSession(HttpResponseMessage responseMessage)
        {
            string content = responseMessage.Content.ReadAsStringAsync().Result;
            IgSessionModel igSession = JsonConvert.DeserializeObject<IgSessionModel>(content);

            if (responseMessage.Headers.TryGetValues("CST", out var values))
            {
                igSession.CST = values.First();
            }
            else
            {
                foreach (var header in responseMessage.Headers)
                {
                    Console.WriteLine($"{header.Key} / {header.Value}");
                    Console.WriteLine(content);
                    Console.WriteLine("Status Code -----------" + responseMessage.StatusCode);
                }

                throw new Exception("Cannot find the CST token in the response headers.");
            }

            if (responseMessage.Headers.TryGetValues("X-SECURITY-TOKEN", out var values2))
            {
                igSession.SecurityToken = values2.First();
            }
            else
            {
                throw new Exception("Cannot find the security token in the response headers.");
            }

            return igSession;
        }

        public IgSessionModel SwitchAccount(IgSessionModel igSession, string accountId)
        {
            string action = "/session";
            HttpClient httpClient = ClientFactory.Create(igSession, 1);
            string json = JsonConvert.SerializeObject(new { accountId = accountId });


            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            string Url = EndPoint() + action;
            var responseMessage = httpClient.PutAsync(new Uri(Url), content).Result;
            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Unable to switch account, reason { responseMessage.ReasonPhrase } Content: {responseMessage.Content.ReadAsStringAsync().Result}.");
            }

            if (responseMessage.Headers.TryGetValues("X-SECURITY-TOKEN", out var values2))
            {
                igSession.SecurityToken = values2.First();
            }
            else
            {
                throw new Exception("Cannot find the security token in the response headers.");
            }

            return igSession;
        }
    }

    public enum EnumIgEnvironment
    {
        Live,
        Demo
    }
}
