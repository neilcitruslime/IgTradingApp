using System;

namespace IgTrading.Models
{
    public class PositionModel
    {
        public string Ticker { get; set;}
        public double Size { get; set; }
        public double Open { get; set; }

        public double Close { get; set; }
        public double OpenValue
        {
            get
            {
                return Size * Open;
            }
        }


        public double CloseValue
        {
            get
            {
                return Size * Close;
            }
        }

        public bool Win
        {
            get
            {
                return CloseValue > OpenValue;
            }
        }

        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public bool Stop { get; set; }

        public bool PassedTest { get; set; } = false;
    }
}