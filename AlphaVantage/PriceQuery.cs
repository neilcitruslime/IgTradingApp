using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using CsvHelper;
using IgTrading.AlphaVantage.Abstract;
using IgTrading.Models;

namespace IgTrading.AlphaVantage
{
    public class PriceQuery : AlphaVantageAbstract<PriceModel>
    {
        public List<PriceModel> Get(string apiKey, string ticker)
        {
            string action = $"{ApiEndPoint}?function=TIME_SERIES_DAILY&symbol={ticker}&apikey={apiKey}&datatype=csv&outputsize=full";

            return GetData(action).OrderByDescending(p => p.Time).ToList();          
        }
    }
}