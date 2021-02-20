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
    public class RsiQuery : AlphaVantageAbstract<RsiModel>
    {
        public List<RsiModel> Get(string apiKey, string ticker, int timePeriod, string type)
        {
            string action = $"{ApiEndPoint}?function=RSI&symbol={ticker}&interval=daily&time_period={timePeriod}&series_type={type}&apikey={apiKey}&datatype=csv";

            return GetData(action).OrderByDescending(p => p.Time).ToList();
        }
    }
}