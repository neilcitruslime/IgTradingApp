using System;
using System.Net.Http;
using System.Text;
using IgTrading.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IgTrading
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
        public string Post(SessionModel igSession, IgOrder igOrder)
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