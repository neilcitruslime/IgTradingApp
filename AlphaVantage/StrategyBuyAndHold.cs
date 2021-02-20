using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IgTrading.AlphaVantage.Abstract;
using IgTrading.Models;

namespace IgTrading.AlphaVantage
{
    public class StrategyBuyAndHold : StrategyAbstract, IStrategy
    {
        public List<PositionModel> RunBackTest(string ticker, SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary, DateTime from, DateTime to, int rsiLow, int rsiHigh, int stopLoss)
        {
            IDictionaryEnumerator myEnumerator =
                     stockDictionary.GetEnumerator();


            List<PositionModel> positions = new List<PositionModel>();

            PositionModel openPosition = null;
            DateTime lastStockDate = GetLastStockPostionDateInFile(stockDictionary, to);
            while (myEnumerator.MoveNext())
            {
                ConsolidatedStockModel model = (ConsolidatedStockModel)myEnumerator.Value;
                DateTime date = (DateTime)myEnumerator.Key;

                if (date >= from && date<=to)
                {                

                    if (date.Date >= from.Date && openPosition == null)
                    {
                        openPosition = new PositionModel() { Open = model.Close, Size = 2000 / model.Close, OpenDate = date};
                        Console.WriteLine($"Buy Date {date} Open {openPosition.Open}");
                    }


                    if (date.Date == lastStockDate && openPosition != null)
                    {
                        openPosition.Close = model.Close;
                        openPosition.CloseDate = date;
                        positions.Add(new PositionModel() { Ticker=ticker, Open = openPosition.Open, Close = openPosition.Close, Size = openPosition.Size, OpenDate = openPosition.OpenDate, CloseDate = openPosition.CloseDate });

                        Console.WriteLine($"Sell Date {date} Force Close End Of Period. Profit {(openPosition.CloseValue - openPosition.OpenValue).ToString("N2")} Open {openPosition.Open} Close {openPosition.Close}");

                        openPosition = null;
                    }
                }
            }

            return positions;
        }

        
    }
}