namespace IgTrading.Ig.Models
{
    public class IgOrderModel
    {
        public string currencyCode { get; set; }
        public string dealReference { get; set; }
        public string direction { get; set; }
        public string epic { get; set; }

        public string expiry { get; set; } = "DFB";
        public bool forceOpen { get; set; } = false;

        public bool guaranteedStop { get; set; } = false;
        public string goodTillDate { get; set; } = "2021/12/31 23:59:59";

        public float level { get; set; }

        public float limitDistance { get; set; }

        public float size { get; set; }

        public float stopDistance { get; set; }

        public string timeInForce { get; set; } = "GOOD_TILL_CANCELLED";

        public string type = "LIMIT";
    }

    public class IgBuyModel
    {
        public string currencyCode { get; set; }
        public string dealReference { get; set; }
        public string direction { get; set; }
        public string epic { get; set; }

        public string expiry { get; set; } = "DFB";
        public bool forceOpen { get; set; } = true;

        public bool guaranteedStop { get; set; } = false;

        public float limitDistance { get; set; }

        public float size { get; set; }

        public float stopDistance { get; set; }

        public string timeInForce { get; set; } = "FILL_OR_KILL";

        public string orderType = "MARKET";

        public float? level = null;
    }

}