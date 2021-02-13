using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using CsvHelper;
using IgTrading.AlphaVantage.Abstract;
using IgTrading.Models;

namespace IgTrading.AlphaVantage
{
    public class RsiQuery : AlphaVantageAbstract
    {
        public List<RsiModel> Get(string apiKey, string ticker, int timePeriod, string type)
        {
            string action = $"{ApiEndPoint}?function=RSI&symbol={ticker}&interval=daily&time_period={timePeriod}&series_type={type}&apikey={apiKey}&datatype=csv";

            HttpClient httpClient = new HttpClient();
            var response = httpClient.GetAsync(new Uri(action)).Result;
           // RsiModel result = JsonConvert.DeserializeObject<RsiModel>(response.Content.ReadAsStringAsync().Result);

            using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<RsiModel>().OrderByDescending(p=>p.Time).ToList();
            }
        }
    }
}