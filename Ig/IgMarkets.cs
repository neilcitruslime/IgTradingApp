using System;
using System.Net.Http;
using System.Threading.Tasks;
using IgTrading.Ig.Models;
using IgTrading.Models;
using Newtonsoft.Json;

namespace IgTrading.Ig
{
    public class IgMarkets
    {
        private IgHttpClient igHttpClient = new IgHttpClient();

        public MarketSearchModel Get(IgSessionModel igSession, string term, bool getDetail)
        {
            string action = "/markets?searchTerm=" + term;


            string json = igHttpClient.Get(igSession, action, 1);
            MarketSearchModel result = JsonConvert.DeserializeObject<MarketSearchModel>(json);

            if (result.Markets != null && getDetail)
            {
                foreach (var market in result.Markets)
                {
                    string ticker = string.Empty;
                    if (!IgEpicMapper.TryLookupCode(market.Epic, out ticker))                    
                    {
                        IgEpicModel epicModel = GetEpic(igSession, market.Epic);
                        if (!string.IsNullOrWhiteSpace(epicModel.instrument.chartCode))
                        {
                            if (epicModel.instrument.country != "US")
                            {
                                if (epicModel.instrument.country=="GB")
                                {
                                    epicModel.instrument.country = "LON";
                                }

                                epicModel.instrument.chartCode += "." + epicModel.instrument.country;
                            }

                            IgEpicMapper.AddCode(market.Epic, epicModel.instrument.chartCode);
                            ticker = epicModel.instrument.chartCode;
                        }
                        
                    }
                    
                    market.Ticker = ticker;
                    Console.WriteLine($"{market.Ticker} {market.InstrumentName}");
                }
            }

            return result;
        }

        public IgEpicModel GetEpic(IgSessionModel igSession, string epic)
        {
            string action = $"/markets/{epic}";
            string json = igHttpClient.Get(igSession, action, 1);

            IgEpicModel result = JsonConvert.DeserializeObject<IgEpicModel>(json);

            return result;
        }
    }
}