using Newtonsoft.Json;

namespace IgTrading.Models
{
    public class LoginModel
    {
        [JsonProperty("identifier")]

        public string Identifier { get; } = "neilcl";

        [JsonProperty("password")]

        public string Password { get; } = "F@eryl1977";
    }
}