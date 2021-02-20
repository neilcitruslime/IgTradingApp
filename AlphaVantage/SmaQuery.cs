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
    public class SmaQuery : AlphaVantageAbstract<SmaModel>
    {

        public List<SmaModel> Get(string apiKey, string ticker, int movingAveragePeriod)
        {
            string action = $"{ApiEndPoint}?function=SMA&symbol={ticker}&interval=daily&time_period={movingAveragePeriod}&series_type=close&apikey={apiKey}&datatype=csv";


            return GetData(action).OrderByDescending(p => p.Time).ToList();

            /*   HttpClient httpClient = new HttpClient();
               var response = httpClient.GetAsync(new Uri(action)).Result;
               // RsiModel result = JsonConvert.DeserializeObject<RsiModel>(response.Content.ReadAsStringAsync().Result);

               using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
               using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
               {
                   return csv.GetRecords<SmaModel>().OrderByDescending(p => p.Time).ToList();
               }*/
        }
    }

}