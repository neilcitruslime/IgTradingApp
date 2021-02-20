using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IgTrading
{
    public static class StringExtentions
    {
         public static string FormatJson(this string json)
        {
            return JValue.Parse(json).ToString(Formatting.Indented);
        }
    }
}