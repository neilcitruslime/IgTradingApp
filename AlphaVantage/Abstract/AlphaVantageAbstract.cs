using System.IO;
using System.Text;

namespace IgTrading.AlphaVantage.Abstract
{
    public class AlphaVantageAbstract
    {
        protected static string ApiEndPoint { get; } = "https://www.alphavantage.co/query";

        protected MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}