using System;
using System.Net.Http;
using System.Text;
using IgTrading.Ig.Models;
using IgTrading.Models;
using Newtonsoft.Json;

namespace IgTrading.Ig
{
    public class IgPositions
    {
        private IgHttpClient igHttpClient = new IgHttpClient();
        public string Get(IgSessionModel igSession)
        {
            string action = "/positions";
        
            IgHttpClient igHttpClient = new IgHttpClient(); 

            
            return igHttpClient.Get(igSession, action,1);
        }

        public string Post(IgSessionModel igSession, IgBuyModel buyOrder)
        {
            string action = "/positions/otc";
            
            StringContent content = new StringContent(JsonConvert.SerializeObject(buyOrder), Encoding.UTF8, "application/json");
                        
            return igHttpClient.Post(igSession, action, 2, content); ;
        }

        public void Put(IgSessionModel igSession, PositionPosition position)
        {
            string action = "/positions/otc/";
            

            StringContent content = new StringContent(JsonConvert.SerializeObject(new { limitLevel = position.LimitLevel, stopLevel = position.StopLevel, trailingStop = false }), Encoding.UTF8, "application/json");

            var returnData = igHttpClient.Put(igSession, action + position.DealId, 2, content);
        }
    }
}