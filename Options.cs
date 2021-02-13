using CommandLine;

namespace IgTrading
{
    [Verb("positions", HelpText = "Show positions.")]
    class PositionOptions
    {
        [Option('a', "account", Required = false, HelpText = "The account you wish to query if you don't set an account all are listed in turn.")]
        public string Account { get; set; }

    }

    [Verb("accounts", HelpText = "List Accounts.")]
    class AccountOptions
    {

    }


    [Verb("order", HelpText = "Place an order for a specific epic.")]
    class OrderOptions
    {
        [Option('a', "account", HelpText = "The account Id", Required = true)]
        public string Account { get; set; }


        [Option("epic", Required = true, HelpText = "The IG Epic code which you wish to place an order for")]
        public string Epic { get; set; }

        [Option("cash", HelpText = "The cash value for the position you wish to open, if this is less than 1 point a position with one point will be opned")]
        public int CashValue { get; set; }

        [Option("direction", HelpText = "buy to create a buy order or sell to create a sell order", Required = true)]
        public string Direction { get; set; }


        [Option("level", HelpText = "The level to execute the order into a position.", Required = true)]
        public float Level { get; set; }


        [Option("limitdistance", HelpText = "The number of points to set as a limit.", Required = true)]
        public float LimitDistance { get; set; }


        [Option("stopdistance", HelpText = "The number of points to set as a stop.", Required = true)]
        public float StopDistance { get; set; }

        [Option("trailing", HelpText = "Set the stop as trailing stop.")]
        public bool Trailing { get; set; } = false;

        [Option('e', "expiry", Required = true, HelpText = "The expiry DFB for daily contracts, e.g. MAR-21 for expirty in March 2021.")]
        public string Expiry { get; set; }
    }


    [Verb("updatestop", HelpText = "Update stop lose on all positions in account.")]
    class UpdateOptions
    {
        [Option('a', "account", HelpText = "The account Id", Required = true)]
        public string Account { get; set; }

        [Option('v', "value", Required = true, HelpText = "The value to set for example 10, this will only adjust is the range increases the current value, for example when adjust a stop if the value here is 10 and the current value is 8% then no change will be made. ")]
        public int Value { get; set; }        
    }
   
    [Verb("alpha", HelpText = "Alpha Vantage Api query")]

    class AlphaOptions
    {
        [Option('t', "ticker", HelpText = "The stock ticker", Required = true)]
        public string Ticker { get; set; }
        
    }

    

    [Verb("quote", HelpText = "Retrieve quotes")]
    class QuoteOptions
    {
        [Option('n', "names", Required = true, HelpText = "The comma seperated list of names you wish to search for.")]
        public string NamesToSearch { get; set; }

        [Option("prefix", Required = false, HelpText = "The epic prefix you wish to filter by.")]

        public string EpicPrefix { get; set; }

        [Option('e', "expiry", Required = true, HelpText = "The expiry DFB for daily contracts, e.g. MAR-21 for expirty in March 2021.")]

        public string Expiry { get; set; }
    }


    [Verb("buy", HelpText = "Preview and execute buy order. Takes the same options as a quote, will ask you to confirm execution.")]    
    public class BuyOptions
    {
        [Option('a', "account", HelpText = "The account Id", Required = true)]
        public string Account { get; set; }

        [Option('n', "names", Required = true, HelpText = "The comma seperated list of names you wish to search for.")]
        public string NamesToSearch { get; set; }

        [Option("prefix", Required = false, HelpText = "The epic prefix you wish to filter by.")]

        public string EpicPrefix { get; set; }

        [Option('e', "expiry", Required = true, HelpText = "The expiry DFB for daily contracts, e.g. MAR-21 for expirty in March 2021.")]

        public string Expiry { get; set; }

        [Option('v', "value", HelpText = "The value to spread across positions in this buy, for example 10,000.", Required = true)]
        public double Value { get; set; }

        [Option("limitdistance", HelpText = "The percentage of bid to set as a limit. For example 25 would be 25 percent.", Required = true)]
        public float LimitDistance { get; set; }


        [Option("stopdistance", HelpText = "he percentage of bid to set as a stop. For example 25 would be 25 percent.", Required = true)]
        public float StopDistance { get; set; }

    }
}