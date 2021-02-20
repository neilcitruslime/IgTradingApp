using System;
using System.Collections;
using System.Collections.Generic;
using IgTrading.Models;

namespace IgTrading.AlphaVantage
{
    public class StrategyRsi : IStrategy
    {
        public List<PositionModel> RunBackTest(string ticker, SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary, DateTime from, DateTime to, int rsiLow, int rsiHigh, int stopLoss)
        {
            IDictionaryEnumerator myEnumerator =
                     stockDictionary.GetEnumerator();


            List<PositionModel> positions = new List<PositionModel>();

            PositionModel openPosition = null;

            while (myEnumerator.MoveNext())
            {
                ConsolidatedStockModel model = (ConsolidatedStockModel)myEnumerator.Value;
                DateTime date = (DateTime)myEnumerator.Key;

                if (date >= from && date<=to)
                {
                    
                    if (model.RsiLow <= rsiLow && openPosition == null && model.Sma200 <= model.Close)
                    {
                        openPosition = new PositionModel() { Open = model.Close, Size = 2000 / model.Close, OpenDate = date};
                        Console.WriteLine($"Buy Date {date} Rsi Low {model.RsiLow} Open {openPosition.Open}");
                        
                    }
                    if (model.RsiHigh >= rsiHigh && openPosition != null)
                    {
                        openPosition.Close = model.Close;
                        openPosition.CloseDate = date;
                        positions.Add(new PositionModel() { Ticker=ticker, Open = openPosition.Open, Close = openPosition.Close, Size = openPosition.Size, OpenDate = openPosition.OpenDate, CloseDate = openPosition.CloseDate });

                        Console.WriteLine($"Sell Date {date} Rsi Low {model.RsiLow} Profit {(openPosition.CloseValue - openPosition.OpenValue).ToString("N2")} Open {openPosition.Open} Close {openPosition.Close}");

                        openPosition = null;
                    }

                    if (stopLoss != 0 && openPosition != null && model.Close <= (openPosition.Open * (1 - ((double)stopLoss / 100))))
                    {
                        openPosition.Close = model.Close;
                        openPosition.CloseDate = date;
                        openPosition.Stop = true;
                        positions.Add(new PositionModel() { Ticker=ticker, Open = openPosition.Open, Close = openPosition.Close, Size = openPosition.Size, OpenDate = openPosition.OpenDate, CloseDate = openPosition.CloseDate, Stop = openPosition.Stop });
                        Console.WriteLine($"Date {date} Rsi Low {model.RsiLow}");
                        Console.WriteLine($"Sell STOP LOSS Profit {(openPosition.CloseValue-openPosition.OpenValue).ToString("N2")}");
                        openPosition = null;

                    }
                }
            }

            return positions;
        }

        
    }
}