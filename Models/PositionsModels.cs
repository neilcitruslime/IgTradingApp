

namespace IgTrading.Models
{
    using System;
    using Newtonsoft.Json;
    public partial class PositionsList
    {
        [JsonProperty("positions")]
        public PositionElement[] Positions { get; set; }
    }

    public partial class PositionElement
    {
        [JsonProperty("position")]
        public PositionPosition Position { get; set; }

        [JsonProperty("market")]
        public Market Market { get; set; }
    }

    public partial class Market
    {
        [JsonProperty("instrumentName")]
        public string InstrumentName { get; set; }

        [JsonProperty("expiry")]
        public string Expiry { get; set; }

        [JsonProperty("epic")]
        public string Epic { get; set; }

        [JsonProperty("instrumentType")]
        public InstrumentType InstrumentType { get; set; }

        [JsonProperty("lotSize")]
        public long LotSize { get; set; }

        [JsonProperty("high")]
        public double? High { get; set; }

        [JsonProperty("low")]
        public double? Low { get; set; }

        [JsonProperty("percentageChange")]
        public double? PercentageChange { get; set; }

        [JsonProperty("netChange")]
        public double? NetChange { get; set; }

        [JsonProperty("bid")]
        public double Bid { get; set; }

        [JsonProperty("offer")]
        public double Offer { get; set; }

        [JsonProperty("updateTime")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty("delayTime")]
        public long DelayTime { get; set; }

        [JsonProperty("streamingPricesAvailable")]
        public bool StreamingPricesAvailable { get; set; }


        [JsonProperty("scalingFactor")]
        public long ScalingFactor { get; set; }
    }

    public partial class PositionPosition
    {
        [JsonProperty("contractSize")]
        public long ContractSize { get; set; }

        [JsonProperty("createdDate")]
        public string CreatedDate { get; set; }

        [JsonProperty("dealId")]
        public string DealId { get; set; }

        [JsonProperty("dealSize")]
        public double DealSize { get; set; }

        [JsonProperty("direction")]
        public Direction Direction { get; set; }

        [JsonProperty("limitLevel")]
        public double LimitLevel { get; set; }

        [JsonProperty("openLevel")]
        public double OpenLevel { get; set; }

        [JsonProperty("currency")]
        public Currency Currency { get; set; }

        [JsonProperty("controlledRisk")]
        public bool ControlledRisk { get; set; }

        [JsonProperty("stopLevel")]
        public double StopLevel { get; set; }

        [JsonProperty("trailingStep")]
        public object TrailingStep { get; set; }

        [JsonProperty("trailingStopDistance")]
        public object TrailingStopDistance { get; set; }

        [JsonProperty("limitedRiskPremium")]
        public object LimitedRiskPremium { get; set; }
    }


    public enum Expiry { Dfb, Jun21, Mar21, Sep21 };

    public enum InstrumentType { Shares, Opt_Shares };

    public enum MarketStatus { Closed, EditsOnly };

    public enum Currency { Gbp };

    public enum Direction { Buy };


}


