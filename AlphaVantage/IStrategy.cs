using System;
using System.Collections.Generic;
using IgTrading.Models;

namespace IgTrading.AlphaVantage
{
    public interface IStrategy
    {
        List<PositionModel> RunBackTest(string ticker, SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary, DateTime from, DateTime to, int rsiLow, int rsiHigh, int stopLoss);
    }
}