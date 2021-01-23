namespace IgTrading.Models
{
    public class EpicModel
    {
        public Instrument instrument { get; set; }
        public Dealingrules dealingRules { get; set; }
        public Snapshot snapshot { get; set; }
    }
    public class Instrument
    {
        public string epic { get; set; }
        public string expiry { get; set; }
        public string name { get; set; }
        public bool forceOpenAllowed { get; set; }
        public bool stopsLimitsAllowed { get; set; }
        public float lotSize { get; set; }
        public string unit { get; set; }
        public string type { get; set; }
        public bool controlledRiskAllowed { get; set; }
        public bool streamingPricesAvailable { get; set; }
        public string marketId { get; set; }
        public Currency[] currencies { get; set; }
        public Margindepositband[] marginDepositBands { get; set; }
        public float margin { get; set; }
        public Slippagefactor slippageFactor { get; set; }
        public Openinghours openingHours { get; set; }
        public Expirydetails expiryDetails { get; set; }
        public Rolloverdetails rolloverDetails { get; set; }
        public string newsCode { get; set; }
        public string chartCode { get; set; }
        public string country { get; set; }
        public object valueOfOnePip { get; set; }
        public object onePipMeans { get; set; }
        public object contractSize { get; set; }
        public string[] specialInfo { get; set; }
    }

    public class Slippagefactor
    {
        public string unit { get; set; }
        public float value { get; set; }
    }

    public class Openinghours
    {
        public Markettime[] marketTimes { get; set; }
    }

    public class Markettime
    {
        public string openTime { get; set; }
        public string closeTime { get; set; }
    }

    public class Expirydetails
    {
        public string lastDealingDate { get; set; }
        public string settlementInfo { get; set; }
    }

    public class Rolloverdetails
    {
        public string lastRolloverTime { get; set; }
        public string rolloverInfo { get; set; }
    }

    public class Currency
    {
        public string code { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public float baseExchangeRate { get; set; }
        public float exchangeRate { get; set; }
        public bool isDefault { get; set; }
    }

    public class Margindepositband
    {
        public float min { get; set; }
        public float? max { get; set; }
        public float margin { get; set; }
    }

    public class Dealingrules
    {
        public Minstepdistance minStepDistance { get; set; }
        public Mindealsize minDealSize { get; set; }
        public Mincontrolledriskstopdistance minControlledRiskStopDistance { get; set; }
        public Minnormalstoporlimitdistance minNormalStopOrLimitDistance { get; set; }
        public Maxstoporlimitdistance maxStopOrLimitDistance { get; set; }
        public string marketOrderPreference { get; set; }
    }

    public class Minstepdistance
    {
        public string unit { get; set; }
        public float value { get; set; }
    }

    public class Mindealsize
    {
        public string unit { get; set; }
        public float value { get; set; }
    }

    public class Mincontrolledriskstopdistance
    {
        public string unit { get; set; }
        public float value { get; set; }
    }

    public class Minnormalstoporlimitdistance
    {
        public string unit { get; set; }
        public float value { get; set; }
    }

    public class Maxstoporlimitdistance
    {
        public string unit { get; set; }
        public float value { get; set; }
    }

    public class Snapshot
    {
        public string marketStatus { get; set; }
        public float netChange { get; set; }
        public float percentageChange { get; set; }
        public string updateTime { get; set; }
        public int delayTime { get; set; }
        public float bid { get; set; }
        public float offer { get; set; }
        public float high { get; set; }
        public float low { get; set; }
        public object binaryOdds { get; set; }
        public int decimalPlacesFactor { get; set; }
        public int scalingFactor { get; set; }
        public float controlledRiskExtraSpread { get; set; }
    }

}