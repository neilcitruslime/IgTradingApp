using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using IgTrading.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IgTrading
{
    public class Program
    {

        [Verb("positions", HelpText = "Show positions.")]
        class PositionOptions
        {
            [Option('a', "account", Required = false, HelpText = "The account you wish to query.")]
            public string Account { get; set; }
        }
        [Verb("accounts", HelpText = "List Accounts.")]
        class AccountOptions
        {
            
        }

        [Verb("updatestop", HelpText = "Update stop lose on all positions in account.")]
        class UpdateOptions
        {
            [Option('s', "stop", Required = true, HelpText = "The stop value to set, above the open. e.g. If the position is above the open the stop will be set from the current position. If the position is below the open no change will be made")]
            public int Stop {get;set;}
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

        static EnumIgEnvironment environment = EnumIgEnvironment.Live;
        static SessionModel session;

        static IgTradingApiConfig igTradingApiConfig;
        static AccountModels accountModels ; 
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings-secure.json", true, true)
                .Build();


            Console.WriteLine($" Hello { config["apiKey"] } !");

            igTradingApiConfig = new IgTradingApiConfig(environment);

            session = igTradingApiConfig.Session();
            IgAccounts igAccounts = new IgAccounts(environment);
            accountModels = igAccounts.Get(session);

            CommandLine.Parser.Default.ParseArguments<PositionOptions, AccountOptions, QuoteOptions>(args)
               .MapResult(
                   (PositionOptions opts) => PositionsList(opts),
                   (AccountOptions opts) => ListAccounts(opts),
                   (QuoteOptions opts) => Quote(opts),
                   errs => 1);


        }


        private static int Quote(QuoteOptions opts)
        {
            MarketSearch(opts.NamesToSearch, opts.Expiry, opts.EpicPrefix);
            return 0;
        }

        static int PositionsList(PositionOptions options)
        {
            SwitchAccount(options.Account);

            Positions();
            return 0;
        }

        static void SwitchAccount(string accountId)
        {
            Console.WriteLine($"Switching to account Id {accountId}");
            if (string.IsNullOrEmpty(accountId) == false)
            {
                session = igTradingApiConfig.SwitchAccount(session, accountId);
            }
        }

        static int ListAccounts(AccountOptions options)
        {
            
            Console.WriteLine($"{"Name".PadRight(15)}Profit\tBalance\t{"Type".PadRight(10)}ID\tAlias");
            foreach (Account account in accountModels.Accounts)
            {
                Console.WriteLine($"{account.AccountName.PadRight(15)} {account.Balance.ProfitLoss} \t{account.Balance.BalanceBalance}\t{account.AccountType.PadRight(10)}{account.AccountId}\t{account.AccountAlias}");
            }
            return 0;
        }

        public static void TestFunction(string[] args)
        {


            //session = igTradingApiConfig.SwitchAccount(session, "IST3L");


            if (false)
            {
                // Console.WriteLine($"Security Token {session.SecurityToken} CST {session.CST}");
                int i = 0;
                switch (args[0])
                {

                    case "order":
                        string prefix = (args.Length == 4 ? args[3] : null);
                        List<Market> toOrder = MarketSearch(args[1], args[2], prefix);


                        foreach (Market market in toOrder)
                        {
                            IgOrder igOrder = new IgOrder { epic = market.Epic, size = 1F, currencyCode = "GBP", level = (float)market.Bid, limitDistance = (float)(market.Bid * 0.2), stopDistance = (float)(market.Bid * 0.1), direction = "BUY", dealReference = $"NMCQ{i}", expiry = market.Expiry };
                            i++;
                            Order(igOrder);
                        }

                        break;

                    case "buy":
                        prefix = (args.Length == 4 ? args[3] : null);
                        List<Market> toBuy = MarketSearch(args[1], args[2], prefix);

                        int positionCount = toBuy.Count();

                        float toInvest = (float)(Convert.ToDouble(args[4]) / positionCount);

                        foreach (Market market in toBuy)
                        {

                            float positionSize = (float)Math.Round(toInvest / market.Bid, 2);
                            if (positionSize < 1)
                            {
                                positionSize = 1;
                            }


                            IgBuy igBuy = new IgBuy
                            {
                                currencyCode = "GBP",
                                dealReference = $"NMCQ{i}",
                                direction = "BUY",
                                epic = market.Epic,
                                expiry = market.Expiry,
                                size = positionSize,
                                limitDistance = (float)(market.Bid * 0.2),
                                stopDistance = (float)(market.Bid * 0.1)
                            };
                            i++;

                            if (args[5] == "EXCUTE")
                            {
                                Buy(igBuy);
                            }
                            else
                            {
                                Console.WriteLine($"Will buy {market.InstrumentName} Position Size { positionSize } for value { Math.Round(positionSize * market.Bid, 2) } ");
                            }

                        }
                        break;


                    case "update":
                        List<PositionElement> positionsToReturn = Positions();
                        double stopPercentage = Convert.ToDouble(args[1]) / 100;

                        foreach (PositionElement positionToAlter in positionsToReturn)
                        {
                            IgPositions igPositions = new IgPositions(environment);

                            positionToAlter.Position.LimitLevel = positionToAlter.Market.Bid * 1.2;
                            double percentageIncrease = (positionToAlter.Market.Bid - positionToAlter.Position.OpenLevel) / positionToAlter.Position.OpenLevel;
                            if (percentageIncrease > stopPercentage)
                            {
                                positionToAlter.Position.StopLevel = positionToAlter.Market.Bid - (positionToAlter.Market.Bid * stopPercentage);
                                igPositions.Put(session, positionToAlter.Position);
                            }
                        }

                        break;
                }
            }




            Console.ReadLine();
        }


        public static List<PositionElement> Positions()
        {
            IgPositions igPositions = new IgPositions(environment);
            string positions = igPositions.Get(session);

            PositionsList positionsList = JsonConvert.DeserializeObject<PositionsList>(positions);
            List<PositionElement> positionsToReturn = new List<PositionElement>();

            double total = 0.0;
            double profit = 0.0;
            Console.WriteLine($"{"Name".PadRight(40)}{"Epic".PadRight(22)}{"Size".PadLeft(6)} { "Bid".PadLeft(8) }   Stop \t    Value \tOpen \t   Profit \tReturn\t  Deal Id ");
            foreach (PositionElement position in positionsList.Positions.OrderBy(p => p.Market.InstrumentName))
            {
                double value = position.Position.DealSize * position.Market.Bid;
                double open = position.Position.OpenLevel * position.Position.DealSize;
                double returnPercentage = (value - open) / open;
                double stopPercentage = (position.Market.Bid - position.Position.StopLevel) / position.Market.Bid;
                total += value;
                profit += value - open;
                Console.WriteLine($"{position.Market.InstrumentName.PadRight(40)}{position.Market.Epic.PadRight(22)}{position.Position.DealSize.ToString().PadLeft(6)}  {position.Market.Bid.ToString("N2").PadLeft(8)}  {stopPercentage.ToString("P1")} \t {Math.Round(value, 2).ToString("N0").PadLeft(8)}\t {Math.Round(open)}\t {Math.Round(value - open, 2).ToString("N2").PadLeft(8)}\t { returnPercentage.ToString("P1")}\t  {position.Position.DealId}");
                positionsToReturn.Add(position);
            }


            Console.WriteLine($"Total {total.ToString("N0")} Profit {profit.ToString("N2")}");
            Console.WriteLine($"Positions {positionsList.Positions.Count()}");

            return positionsToReturn;
        }

        public static List<Market> MarketSearch(string query, string expiry, string prefix)
        {
            IgMarkets igMarkets = new IgMarkets(environment);
            string[] quotes = query.Split(',');

            int count = 0;
            List<Market> parsedList = new List<Market>();

            foreach (string ticker in quotes)
            {
                MarketSearchModel marketSearch = igMarkets.Get(session, ticker);



                foreach (Market market in marketSearch.Markets)
                {
                    if (market.Expiry == expiry && (string.IsNullOrEmpty(prefix) || market.Epic.StartsWith(prefix)))
                    {
                        parsedList.Add(market);
                        count++;
                    }
                }


            }

            parsedList = parsedList.OrderBy(p => p.InstrumentName).ToList();
            parsedList.ForEach(market => Console.WriteLine($"{market.InstrumentName.PadLeft(70)} // {market.Epic.ToString().PadRight(30)} Bid {market.Bid}\tExpiry {market.Expiry}"));


            Console.WriteLine($"Found {count} stocks.");

            return parsedList;
        }

        public static void Order(IgOrder igOrder)
        {
            IgWorkingOrder igWorkingOrder = new IgWorkingOrder(environment);
            var response = igWorkingOrder.Post(session, igOrder);
            Console.WriteLine(response);
            response = new IgConfirms(environment).GetConfirms(session, igOrder.dealReference);
            Console.WriteLine(response);
        }

        public static void Buy(IgBuy igBuy)
        {
            IgPositions igPosition = new IgPositions(environment);
            var response = igPosition.Post(session, igBuy);
            Console.WriteLine(response);
            response = new IgConfirms(environment).GetConfirms(session, igBuy.dealReference);
            Console.WriteLine(response);
        }

        public static string FormatJson(string json)
        {
            return JValue.Parse(json).ToString(Formatting.Indented);
        }

    }
}
