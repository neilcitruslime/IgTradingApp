namespace IgTrading.Models
{
    using System;
    using CsvHelper.Configuration.Attributes;

    public partial class RsiModel
    {
        [Name("RSI")]
        public double Rsi { get; set; }
        
        [Name("time")]
        public DateTime Time { get; set; }
    }
}
