namespace IgTrading.Ig
{
    using IgTrading.Ig.Models;
    using IgTrading.Models;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;

    public class IgTradingApiConfig
    {
        private static string ApiEndPoint = "gateway/deal";
        private static string Demo = "https://demo-api.ig.com/";
        private static string Live = "https://api.ig.com/";

        public static EnumIgEnvironment Environment;
        private readonly LoginModel login;

        public IgTradingApiConfig(LoginModel login)
        {            
            this.login = login;
        }

        public static string EndPoint()
        {
            if (Environment == EnumIgEnvironment.Live)
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
            string result = string.Empty;
            HttpResponseMessage responseMessage =null;
            while (string.IsNullOrWhiteSpace(result))
            {
                responseMessage = httpClient.PostAsync(new Uri(Url), content).Result;
                result = responseMessage.Content.ReadAsStringAsync().Result;
                if (result.Contains("error.public-api.exceeded-api-key-allowance"))
                {
                    result = string.Empty;
                    Thread.Sleep(1000 * 60);
                }
            }

            IgSessionModel igSession = GetSession(responseMessage, result);

            return igSession;
        }

        private static IgSessionModel GetSession(HttpResponseMessage responseMessage, string content)
        {

            IgSessionModel igSession = JsonConvert.DeserializeObject<IgSessionModel>(content);
  
            if (responseMessage.Headers.TryGetValues("CST", out var values))
            {
                igSession.CST = values.First();
            }
            else
            {  
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
