namespace IgTrading.Ig.Models
{
    using IgTrading.Models;
    using Newtonsoft.Json;
    using System;

    public partial class IgSessionModel
    {
        [JsonProperty("accountType")]
        public string AccountType { get; set; }

        [JsonProperty("accountInfo")]
        public AccountInfo AccountInfo { get; set; }

        [JsonProperty("currencyIsoCode")]
        public string CurrencyIsoCode { get; set; }

        [JsonProperty("currencySymbol")]
        public string CurrencySymbol { get; set; }

        [JsonProperty("currentAccountId")]
        public string CurrentAccountId { get; set; }

        [JsonProperty("lightstreamerEndpoint")]
        public Uri LightstreamerEndpoint { get; set; }

        [JsonProperty("accounts")]
        public Account[] Accounts { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("timezoneOffset")]
        public long TimezoneOffset { get; set; }

        [JsonProperty("hasActiveDemoAccounts")]
        public bool HasActiveDemoAccounts { get; set; }

        [JsonProperty("hasActiveLiveAccounts")]
        public bool HasActiveLiveAccounts { get; set; }

        [JsonProperty("trailingStopsEnabled")]
        public bool TrailingStopsEnabled { get; set; }

        [JsonProperty("reroutingEnvironment")]
        public object ReroutingEnvironment { get; set; }

        [JsonProperty("dealingEnabled")]
        public bool DealingEnabled { get; set; }
        public string CST { get; internal set; }
        public string SecurityToken { get; internal set; }
    }

    public partial class AccountInfo
    {
        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("deposit")]
        public double Deposit { get; set; }

        [JsonProperty("profitLoss")]
        public double ProfitLoss { get; set; }

        [JsonProperty("available")]
        public double Available { get; set; }
    }

}