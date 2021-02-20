using IgTrading.Ig.Models;
using IgTrading.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace IgTrading.Ig
{
    public class IgWorkingOrder
    {
        private readonly LoginModel login;
        private EnumIgEnvironment environment;

        public IgWorkingOrder(EnumIgEnvironment environment, LoginModel login)
        {
            this.login = login;
            this.environment = environment;
        }
        public string Post(IgSessionModel igSession, IgOrderModel igOrder)
        {
            string action = "/workingorders/otc";
            HttpClient httpClient = ClientFactory.Create(igSession, 2);
            IgTradingApiConfig igTradingApiConfig = new IgTradingApiConfig(environment, login);

            string json = JsonConvert.SerializeObject(igOrder);
            Console.WriteLine(json.FormatJson());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(new Uri(igTradingApiConfig.EndPoint() + action), content).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }


    }
}