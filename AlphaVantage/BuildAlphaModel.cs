using System;
using System.Collections.Generic;
using System.Threading;
using IgTrading.Models;
using IgTrading.Store;
using Newtonsoft.Json;

namespace IgTrading.AlphaVantage
{
    public class BuildAlphaModel
    {
        public SortedDictionary<DateTime, ConsolidatedStockModel> Build(string alphaKey, string ticker)
        {
           
            SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary;
            string savedJson = FileStore.GetFile(ticker);
            if (savedJson == string.Empty)
            {
                stockDictionary = GetData(alphaKey, ticker);
                FileStore.StoreFile(ticker, JsonConvert.SerializeObject(stockDictionary), DateTime.Now);
            }else
            {
                stockDictionary  = JsonConvert.DeserializeObject<SortedDictionary<DateTime, ConsolidatedStockModel>>(savedJson);
            }
            return stockDictionary;
        }
        private SortedDictionary<DateTime, ConsolidatedStockModel> GetData(string alphaKey, string ticker)
        {
            List<PriceModel> alphaPriceResult = new PriceQuery().Get(alphaKey, ticker);
            List<RsiModel> alphaRsiResult = new RsiQuery().Get(alphaKey, ticker, 4, "low");
            List<RsiModel> alphaRsiResultHigh = new RsiQuery().Get(alphaKey, ticker, 4, "high");
            List<SmaModel> alphaSmaResult = new SmaQuery().Get(alphaKey, ticker, 200);            

            SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary = new SortedDictionary<DateTime, ConsolidatedStockModel>();

            foreach (RsiModel rsiModel in alphaRsiResult)
            {
                ConsolidatedStockModel consolidatedStock;
                if (!stockDictionary.ContainsKey(rsiModel.Time))
                {
                    consolidatedStock = new ConsolidatedStockModel();
                    consolidatedStock.RsiLow = rsiModel.Rsi;
                    stockDictionary.Add(rsiModel.Time, consolidatedStock);
                }
                else
                {
                    stockDictionary[rsiModel.Time].RsiLow = rsiModel.Rsi;
                }
            }


            foreach (RsiModel rsiModel in alphaRsiResultHigh)
            {
                ConsolidatedStockModel consolidatedStock;
                if (!stockDictionary.ContainsKey(rsiModel.Time))
                {
                    consolidatedStock = new ConsolidatedStockModel();
                    consolidatedStock.RsiHigh = rsiModel.Rsi;
                    stockDictionary.Add(rsiModel.Time, consolidatedStock);
                }
                else
                {
                    stockDictionary[rsiModel.Time].RsiHigh = rsiModel.Rsi;
                }
            }

            foreach (SmaModel smaModel in alphaSmaResult)
            {
                ConsolidatedStockModel consolidatedStock;
                if (!stockDictionary.ContainsKey(smaModel.Time))
                {
                    consolidatedStock = new ConsolidatedStockModel();
                    consolidatedStock.Sma200 = smaModel.Sma;
                    stockDictionary.Add(smaModel.Time, consolidatedStock);
                }
                else
                {
                    stockDictionary[smaModel.Time].Sma200 = smaModel.Sma;
                }
            }

            foreach (PriceModel priceModel in alphaPriceResult)
            {
                ConsolidatedStockModel consolidatedStock;
                if (!stockDictionary.ContainsKey(priceModel.Time))
                {
                    consolidatedStock = new ConsolidatedStockModel();
                    consolidatedStock.Close = priceModel.Close;
                    stockDictionary.Add(priceModel.Time, consolidatedStock);
                }
                else
                {
                    stockDictionary[priceModel.Time].Close = priceModel.Close;
                }
            }

            return stockDictionary;
        }

    }
}