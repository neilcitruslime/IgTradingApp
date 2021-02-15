using System.Collections.Generic;
using Newtonsoft.Json;

namespace IgTrading.Models
{    
    public class AccountModels
    {
        [JsonProperty("accounts")]
        public List<Account> Accounts { get; set; }
    }

    public partial class Account
    {
        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        [JsonProperty("accountAlias")]
        public object AccountAlias { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("accountType")]
        public string AccountType { get; set; }

        [JsonProperty("preferred")]
        public bool Preferred { get; set; }

        [JsonProperty("balance")]
        public Balance Balance { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("canTransferFrom")]
        public bool CanTransferFrom { get; set; }

        [JsonProperty("canTransferTo")]
        public bool CanTransferTo { get; set; }
    }

    public partial class Balance
    {
        [JsonProperty("balance")]
        public double BalanceBalance { get; set; }

        [JsonProperty("deposit")]
        public double Deposit { get; set; }

        [JsonProperty("profitLoss")]
        public double ProfitLoss { get; set; }

        [JsonProperty("available")]
        public double Available { get; set; }
    }
}