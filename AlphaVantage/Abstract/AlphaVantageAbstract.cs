using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace IgTrading.AlphaVantage.Abstract
{
    public class AlphaVantageAbstract<T>
    {
        private const int TwentySeconds = 1000 * 20;

        protected static string ApiEndPoint { get; } = "https://www.alphavantage.co/query";

        protected List<T> GetData(string action)
        {
            string content = string.Empty;

            content = MakeApiCall(action, content);
            return ProcessCsvIntoList(content);
        }

        private static List<T> ProcessCsvIntoList(string content)
        {
            CsvConfiguration configuration = new CsvConfiguration(new CultureInfo("en-US"));
            var reader = new StringReader(content);
            using (CsvReader csv = new CsvReader(reader, configuration))
            {
                return csv.GetRecords<T>().ToList();
            }
        }

        private string MakeApiCall(string action, string content)
        {
            while (string.IsNullOrWhiteSpace(content))
            {
                content = MakeRequest(action);
                if (content.Contains("higher API call frequency"))
                {
                    content = string.Empty;
                    Thread.Sleep(TwentySeconds);
                }
            }

            return content;
        }

        private string MakeRequest(string action)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;

            response = httpClient.GetAsync(new Uri(action)).Result;

            string content = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Failed to read data from Alpha Vantage API {response.StatusCode}. Message: {content}");
            }
            return content;
        }
    }
}