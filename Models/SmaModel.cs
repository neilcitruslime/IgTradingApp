using System;
using CsvHelper.Configuration.Attributes;

namespace IgTrading.Models
{
    public class SmaModel
    {
        [Name("SMA")]
        public double Sma { get; set; }
        
        [Name("time")]
        public DateTime Time { get; set; }

    }
}