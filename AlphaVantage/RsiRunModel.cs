using System;
using System.Collections;
using System.Collections.Generic;
using IgTrading.Models;

namespace IgTrading.AlphaVantage
{
    public class StrategyRsi
    {
        public List<PositionModel> RunBackTest(SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary, int rsiLow, int rsiHigh, int stopLoss)
        {
            IDictionaryEnumerator myEnumerator =
                     stockDictionary.GetEnumerator();


            List<PositionModel> positions = new List<PositionModel>();

            PositionModel openPosition = null;

            while (myEnumerator.MoveNext())
            {
                ConsolidatedStockModel model = (ConsolidatedStockModel)myEnumerator.Value;
                DateTime date = (DateTime)myEnumerator.Key;

                if (date >= new DateTime(2020, 6, 1))
                {
                    if (model.RsiLow <= rsiLow && openPosition == null && model.Sma200 <= model.Close)
                    {
                        openPosition = new PositionModel() { Open = model.Close, Size = 2000 / model.Close, OpenDate = date };
                    }
                    if (model.RsiHigh >= rsiHigh && openPosition != null)
                    {
                        openPosition.Close = model.Close;
                        openPosition.CloseDate = date;
                        positions.Add(new PositionModel() { Open = openPosition.Open, Close = openPosition.Close, Size = openPosition.Size, OpenDate = openPosition.OpenDate, CloseDate = openPosition.CloseDate });
                        openPosition = null;
                    }

                    if (stopLoss !=0 && openPosition != null && model.Close <= (openPosition.Open * (1 - ((double)stopLoss/100))))
                    {
                        openPosition.Close = model.Close;
                        openPosition.CloseDate = date;
                        openPosition.Stop = true;
                        positions.Add(new PositionModel() { Open = openPosition.Open, Close = openPosition.Close, Size = openPosition.Size, OpenDate = openPosition.OpenDate, CloseDate = openPosition.CloseDate, Stop = openPosition.Stop });
                        openPosition = null;
                    }
                }
            }

            return positions;
        }
    }
}