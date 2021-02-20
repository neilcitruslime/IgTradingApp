using Newtonsoft.Json;

namespace IgTrading.Models
{
    public class LoginModel
    {
        [JsonProperty("identifier")]

        public string Identifier { get; set;} 

        [JsonProperty("password")]

        public string Password { get; set;} 
    }
}