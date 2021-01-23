using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using IgTrading.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IgTrading
{
    public class Program
    {
        public static LoginModel login = new LoginModel();

        static EnumIgEnvironment environment = EnumIgEnvironment.Live;
        static SessionModel session;

        static IgTradingApiConfig igTradingApiConfig;
        static AccountModels accountModels;
        static void Main(string[] args)
        {
            ReadConfiguration();

            igTradingApiConfig = new IgTradingApiConfig(environment, login);

            session = igTradingApiConfig.Session();
            IgAccounts igAccounts = new IgAccounts(environment, login);
            accountModels = igAccounts.Get(session);

            CommandLine.Parser.Default.ParseArguments<PositionOptions, AccountOptions, QuoteOptions, OrderOptions, UpdateOptions>(args)
               .MapResult(
                   (PositionOptions opts) => PositionsList(opts),
                   (AccountOptions opts) => ListAccounts(opts),
                   (QuoteOptions opts) => Quote(opts),
                   (OrderOptions opts) => Order(opts),
                   (UpdateOptions opts) => Update(opts),
                   errs => 1);
        }

        private static void ReadConfiguration()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings-secure.json", true, true)
                .Build();

            ClientFactory.ApiKey = config["apiKey"];
            login.Identifier = config["username"];
            login.Password = config["password"];
        }

        private static int Quote(QuoteOptions opts)
        {
            MarketSearch(opts.NamesToSearch, opts.Expiry, opts.EpicPrefix);
            return 0;
        }


        private static int Order(OrderOptions options)
        {
            SwitchAccount(options.Account);

            float positionSize = 1F;

            IgOrder igOrder = new IgOrder { epic = options.Epic, size = positionSize, currencyCode = "GBP", level = options.Level, limitDistance = options.LimitDistance, stopDistance = options.StopDistance, direction = options.Direction.ToUpper(), dealReference = $"NMCQReference{1}", expiry = options.Expiry };

            IgMarkets igMarkets = new IgMarkets(environment, login);
            EpicModel epic = igMarkets.GetEpic(session, options.Epic);

            Console.WriteLine($"Market Code {epic.instrument.newsCode} {epic.instrument.marketId} {epic.instrument.chartCode} Bid: {epic.snapshot.bid}");
            PlaceOrder(igOrder);
            return 0;

        }

        private static int Update(UpdateOptions options)
        {
            SwitchAccount(options.Account);

            List<PositionElement> positionsToReturn = Positions();
            double stopPercentage = Convert.ToDouble(options.Value) / 100;

            foreach (PositionElement positionToAlter in positionsToReturn)
            {
                IgPositions igPositions = new IgPositions(environment, login);

                double percentageIncrease = (positionToAlter.Market.Bid - positionToAlter.Position.OpenLevel) / positionToAlter.Position.OpenLevel;
                if (percentageIncrease > stopPercentage)
                {
                    double plannedStop = positionToAlter.Market.Bid - (positionToAlter.Market.Bid * stopPercentage);
                    Console.WriteLine($"Postion to alter {positionToAlter.Market.InstrumentName} Current Stop {positionToAlter.Position.StopLevel} Planned new stop {plannedStop} ");
                    positionToAlter.Position.StopLevel = plannedStop;
                    igPositions.Put(session, positionToAlter.Position);
                }
            }
            return 0;
        }
        static int PositionsList(PositionOptions options)
        {

            List<string> accountIds = new List<string>();

            if (string.IsNullOrWhiteSpace(options.Account) == false)
            {
                accountIds.Add(options.Account);
            }
            else
            {
                accountModels.Accounts.ForEach(p => accountIds.Add(p.AccountId));
            }

            foreach (string accountId in accountIds)
            {
                SwitchAccount(accountId);
                Positions();
            }

            return 0;
        }

        static void SwitchAccount(string accountId)
        {
            Console.WriteLine($"Switching to account Id {accountId} from {session.CurrentAccountId} ");
            if (string.IsNullOrEmpty(accountId) == false && session.CurrentAccountId != accountId)
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


            // Console.WriteLine($"Security Token {session.SecurityToken} CST {session.CST}");
            int i = 0;
            switch (args[0])
            {



                case "buy":
                    string prefix = (args.Length == 4 ? args[3] : null);
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


            }





            Console.ReadLine();
        }


        public static List<PositionElement> Positions()
        {
            IgPositions igPositions = new IgPositions(environment, login);
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
            IgMarkets igMarkets = new IgMarkets(environment, login);
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
            parsedList.ForEach(market => Console.WriteLine($"{market.InstrumentName.PadLeft(70)} // {market.Epic.ToString().PadRight(30)} Bid {market.Bid}\tExpiry {market.Expiry} Ticker { market.EpicModel.instrument.chartCode }"));


            Console.WriteLine($"Found {count} stocks.");

            return parsedList;
        }

        public static void PlaceOrder(IgOrder igOrder)
        {
            IgWorkingOrder igWorkingOrder = new IgWorkingOrder(environment, login);
            var response = igWorkingOrder.Post(session, igOrder);
            Console.WriteLine(response);
            response = new IgConfirms(environment, login).GetConfirms(session, igOrder.dealReference);
            Console.WriteLine(response);
        }

        public static void Buy(IgBuy igBuy)
        {
            IgPositions igPosition = new IgPositions(environment, login);
            var response = igPosition.Post(session, igBuy);
            Console.WriteLine(response);
            response = new IgConfirms(environment, login).GetConfirms(session, igBuy.dealReference);
            Console.WriteLine(response);
        }
    }
}
