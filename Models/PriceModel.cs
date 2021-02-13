using System;
using CsvHelper.Configuration.Attributes;

namespace IgTrading.Models
{
    public class PriceModel
    {
        [Name("open")]
        public double Open { get; set; }

        [Name("high")]
        public double High { get; set; }

        [Name("low")]
        public double Low { get; set; }

        [Name("close")]
        public double Close { get; set; }

        [Name("timestamp")]
        public DateTime Time { get; set; }
    }
}