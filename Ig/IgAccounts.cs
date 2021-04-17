using System;
using System.Net.Http;
using IgTrading.Ig.Models;
using IgTrading.Models;
using Newtonsoft.Json;


namespace IgTrading.Ig
{
    public class IgAccounts
    {
        private EnumIgEnvironment environment;
        private readonly LoginModel login;

        public IgAccounts(EnumIgEnvironment environment, LoginModel login)
        {
            this.login = login;
            this.environment = environment;
        }

        public IgAccountModels Get(IgSessionModel igSession)
        {
            string action = "/accounts";
            IgHttpClient igHttpClient = new IgHttpClient(); 
            string json = igHttpClient.Get(igSession, action,1);

            return JsonConvert.DeserializeObject<IgAccountModels>(json);
        }
    }
}