
using IgTrading.Ig.Models;
using IgTrading.Models;
using System;
using System.Net.Http;
using System.Threading;

namespace IgTrading.Ig
{
    public class IgHttpClient
    {
        public IgHttpClient()
        {

        }

        public static DateTime lastCallTime = DateTime.Now;

        public string Get(IgSessionModel igSession, string action, int apiVersion)
        {
            using HttpClient httpClient = ClientFactory.Create(igSession, apiVersion);

            string result = string.Empty;
            while (string.IsNullOrWhiteSpace(result))
            {
                HttpResponseMessage response = httpClient.GetAsync(GetUri(action)).Result;
                result = response.Content.ReadAsStringAsync().Result;
                if (result.Contains("error.public-api.exceeded-api-key-allowance"))
                {
                    result = string.Empty;
                    Console.WriteLine("Ig API Backoff");
                    Thread.Sleep(1000 * 30);
                }
            }
             return result;                       
        }

        private Uri GetUri(string action)
        {
            return new Uri(IgTradingApiConfig.EndPoint() + action);
        }

        public string Post(IgSessionModel igSession, string action, int apiVersion, StringContent content)
        {
             using HttpClient httpClient = ClientFactory.Create(igSession, apiVersion);

            string result = string.Empty;
            while (string.IsNullOrWhiteSpace(result))
            {
                HttpResponseMessage response = httpClient.PostAsync(GetUri(action), content).Result;
                result = response.Content.ReadAsStringAsync().Result;
                if (result.Contains("error.public-api.exceeded-api-key-allowance"))
                {
                    result = string.Empty;
                    Thread.Sleep(1000 * 60);
                }
            }
             return result;
        }

        public string Put(IgSessionModel igSession, string action, int apiVersion, StringContent content)
        {
             using HttpClient httpClient = ClientFactory.Create(igSession, apiVersion);

            string result = string.Empty;
            while (string.IsNullOrWhiteSpace(result))
            {
                HttpResponseMessage response = httpClient.PutAsync(GetUri(action), content).Result;
                result = response.Content.ReadAsStringAsync().Result;
                if (result.Contains("error.public-api.exceeded-api-key-allowance"))
                {
                    result = string.Empty;
                    Thread.Sleep(1000 * 60);
                }
            }
             return result;
        }
         
    }
}
