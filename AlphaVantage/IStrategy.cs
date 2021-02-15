using System;
using System.Collections.Generic;
using IgTrading.Models;

namespace IgTrading.AlphaVantage
{
    public interface IStrategy
    {
        List<PositionModel> RunBackTest(SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary, int rsiLow, int rsiHigh, int stopLoss);
    }
}