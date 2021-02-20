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
    public class PriceQuery : AlphaVantageAbstract
    {
        public List<PriceModel> Get(string apiKey, string ticker)
        {
            string action = $"{ApiEndPoint}?function=TIME_SERIES_DAILY&symbol={ticker}&apikey={apiKey}&datatype=csv&outputsize=full";

            HttpClient httpClient = new HttpClient();
            var response = httpClient.GetAsync(new Uri(action)).Result;
           // RsiModel result = JsonConvert.DeserializeObject<RsiModel>(response.Content.ReadAsStringAsync().Result);

            using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<PriceModel>().OrderByDescending(p=>p.Time).ToList();
            }
        }
    }
}