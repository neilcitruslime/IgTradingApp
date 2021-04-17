using System;
using System.Collections;
using System.Collections.Generic;
using IgTrading.Models;

namespace IgTrading.AlphaVantage
{
    public class StrategyRsiBuyOnStrength : IStrategy
    {
        public List<PositionModel> RunBackTest(string ticker, SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary, DateTime from, DateTime to, int rsiLow, int rsiHigh, int stopLoss)
        {
            IDictionaryEnumerator myEnumerator =
                     stockDictionary.GetEnumerator();


            List<PositionModel> positions = new List<PositionModel>();

            PositionModel openPosition = null;
            bool rsiDropped = false;
            while (myEnumerator.MoveNext())
            {
                ConsolidatedStockModel currentStock = (ConsolidatedStockModel)myEnumerator.Value;
                DateTime date = (DateTime)myEnumerator.Key;

                if (date >= from && date<=to)                
                {
                    if (currentStock.RsiLow <= rsiLow && openPosition == null && currentStock.Sma200 <= currentStock.Close)
                    {
                        rsiDropped = true;
                    }

                    if (rsiDropped && currentStock.RsiLow >= rsiLow && openPosition == null)
                    {
                        rsiDropped = false;
                        // open position
                        openPosition = new PositionModel() { Open = currentStock.Close, Size = 2000 / currentStock.Close, OpenDate = date, Ticker=ticker };
                    }

                    if (openPosition != null && currentStock.RsiHigh >= rsiHigh)
                    {
                        // close position
                        openPosition.Close = currentStock.Close;
                        openPosition.CloseDate = date;
                        positions.Add(new PositionModel() { Ticker=ticker, Open = openPosition.Open, Close = openPosition.Close, Size = openPosition.Size, OpenDate = openPosition.OpenDate, CloseDate = openPosition.CloseDate });
                        openPosition = null;
                    }

                    if (stopLoss != 0 && openPosition != null && currentStock.Close <= openPosition.Open * (1 - (double)stopLoss / 100))
                    {
                        // close position
                        openPosition.Close = currentStock.Close;
                        openPosition.CloseDate = date;
                        openPosition.Stop = true;
                        positions.Add(new PositionModel() { Ticker=ticker, Open = openPosition.Open, Close = openPosition.Close, Size = openPosition.Size, OpenDate = openPosition.OpenDate, CloseDate = openPosition.CloseDate, Stop = openPosition.Stop });
                        openPosition = null;
                    }
                }
                
            }

            
            return positions;
        }
    }
}