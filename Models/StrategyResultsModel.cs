namespace IgTrading.Models
{
    public class StrategyResultsModel
    {
        public string Ticker { get; set; }
        public double Profit { get; set; }
        public int Stops { get; set; }

        public int TotalPositions { get; set; }

        public double WinRate { get; set; }

        public int MaxDays { get; set; }

        public int PositionsTaken {get;set;}
        public double AverageDays {get;set;}

        public string Name {get;set;}
        public double TotalDays { get; internal set; }
    }
}