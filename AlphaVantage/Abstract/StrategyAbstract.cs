using System;
using System.Collections.Generic;
using System.Linq;
using IgTrading.Models;

namespace IgTrading.AlphaVantage.Abstract
{
    public class StrategyAbstract
    {
        protected DateTime GetLastStockPostionDateInFile(SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary, DateTime to)
        {
            DateTime lastStockDate = stockDictionary.Keys.Where(p => p.Date <= to).Max();
            return lastStockDate;
        }
    }
}