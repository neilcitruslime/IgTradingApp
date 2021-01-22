using System.Net.Http;
using IgTrading.Models;

namespace IgTrading
{ 
    public class ClientFactory
    {
        public static string ApiKey { set; get; } 

        public static HttpClient Create()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-IG-API-KEY", ApiKey);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json; charset=UTF-8");
            httpClient.DefaultRequestHeaders.Add("Version", "1");
            return httpClient;
        }            

        public static HttpClient Create(SessionModel igSession, int version)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-IG-API-KEY", ApiKey);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json; charset=UTF-8");
            httpClient.DefaultRequestHeaders.Add("Version", version.ToString());
            httpClient.DefaultRequestHeaders.Add("CST", igSession.CST);
            httpClient.DefaultRequestHeaders.Add("X-SECURITY-TOKEN", igSession.SecurityToken);
            
            return httpClient;
        }            

    }
}