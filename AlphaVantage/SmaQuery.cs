using IgTrading.AlphaVantage.Abstract;
using IgTrading.Models;
using System.Collections.Generic;
using System.Linq;

namespace IgTrading.AlphaVantage
{
    public class SmaQuery : AlphaVantageAbstract<SmaModel>
    {

        public List<SmaModel> Get(string apiKey, string ticker, int movingAveragePeriod)
        {
            string action = $"{ApiEndPoint}?function=SMA&symbol={ticker}&interval=daily&time_period={movingAveragePeriod}&series_type=close&apikey={apiKey}&datatype=csv";


            return GetData(action).OrderByDescending(p => p.Time).ToList();
        }
    }
}