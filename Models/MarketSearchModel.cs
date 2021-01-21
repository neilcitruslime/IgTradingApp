using Newtonsoft.Json;

namespace IgTrading.Models
{
    public class MarketSearchModel
    {
        [JsonProperty("markets")]
        public Market[] Markets { get; set; }
    }
}