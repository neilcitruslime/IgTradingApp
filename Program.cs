﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CommandLine;
using CsvHelper;
using IgTrading.AlphaVantage;
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
        static string alphaKey = string.Empty;

        static IgTradingApiConfig igTradingApiConfig;
        static AccountModels accountModels;
        static void Main(string[] args)
        {
            ReadConfiguration();

            igTradingApiConfig = new IgTradingApiConfig(environment, login);

            session = igTradingApiConfig.Session();
            IgAccounts igAccounts = new IgAccounts(environment, login);
            accountModels = igAccounts.Get(session);

            CommandLine.Parser.Default.ParseArguments<PositionOptions, AccountOptions, QuoteOptions, OrderOptions, UpdateOptions, BuyOptions, AlphaOptions>(args)
               .MapResult(
                   (PositionOptions opts) => PositionsList(opts),
                   (AccountOptions opts) => ListAccounts(opts),
                   (QuoteOptions opts) => Quote(opts),
                   (OrderOptions opts) => Order(opts),
                   (UpdateOptions opts) => Update(opts),
                   (BuyOptions opts) => Buy(opts),  
                   (AlphaOptions opts) => Alpha(opts),
                   errs => 1);
        }

        private static int Alpha(AlphaOptions opts)
        {

            List<string> tickers = opts.Ticker.Trim(',').Split(',').ToList();

            BuildAlphaModel buildAlphaModel = new BuildAlphaModel();

            Dictionary<string, SortedDictionary<DateTime, ConsolidatedStockModel>> stockDataLibrary = new Dictionary<string, SortedDictionary<DateTime, ConsolidatedStockModel>>();

            int count = tickers.Count; int i = 0;
            foreach (string ticker in tickers)
            {
                try
                {
                    SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary;
                    stockDictionary = buildAlphaModel.Build(alphaKey, ticker);
                    stockDataLibrary.Add(ticker, stockDictionary);
                    Console.WriteLine($"Got Data for {ticker}");
                }
                catch
                {
                    Console.WriteLine($"Failed to get ticker '{ticker}'");
                }

                i++;

            }
            List<StrategyResultsModel> finalResults = new List<StrategyResultsModel>();

            //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, 25, 55, 0, new StrategyRsi()));
            // //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, 25, 55, 25, new StrategyRsi()));

            DateTime from = new DateTime(2021, 1, 1);
            DateTime to = new DateTime(2021, 2, 18);

            finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 25, 75, 0, new StrategyBuyAndHold()));
            //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 25, 55, 0, new StrategyRsi()));

            // //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, 25, 65, 25, new StrategyRsi()));

            //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from,to,45, 85, 0, new StrategyRsi()));
            //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from,to,45, 55, 0, new StrategyRsiHold()));

            finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 45, 85, 10, new StrategyRsiBuyOnStrength()));
            finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 45, 85, 0, new StrategyRsiBuyOnStrength()));
            
            // finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 35, 85, 0, new StrategyRsiBuyOnStrength()));
            // finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 25, 85, 0, new StrategyRsiBuyOnStrength()));

            // finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 45, 55, 0, new StrategyRsiBuyOnStrength()));
            // finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 35, 55, 0, new StrategyRsiBuyOnStrength()));
            // finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 25, 55, 0, new StrategyRsiBuyOnStrength()));

            //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 45, 85, 0, new StrategyRsiBuyOnStrength()));

            //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 45, 85, 0, new StrategyRsi()));

            // Console.WriteLine("Start Advantaged");
            // finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, 25, 55, 0, new StrategyRsiHold()));
            // //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, 25, 55, 25, new StrategyRsiHold()));

            // finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, 25, 65, 0, new StrategyRsiHold()));
            // //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, 25, 65, 25, new StrategyRsiHold()));

            //finalResults.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 25, 75, 0, new StrategyRsiHold()));
            //.AddRange(RunStrategy(tickers, stockDataLibrary, from, to, 45, 85, 0, new StrategyRsiHold()));

            PrintStrategyHeader();
            finalResults.Where(p => p.Ticker != "SUMMARY").OrderByDescending(p => p.Ticker).ToList().ForEach(p => PrintStrategyResults(p));

            finalResults.Where(p => p.Ticker == "SUMMARY").ToList().ForEach(p => PrintStrategyResults(p));


            return 0;
        }

        private static List<StrategyResultsModel> RunStrategy(
            List<string> tickers,
            Dictionary<string, SortedDictionary<DateTime, ConsolidatedStockModel>> stockDataLibrary,
            DateTime from, DateTime to,
            int rsiLow,
            int rsiHigh,
            int stopLoss,
            IStrategy strategy)
        {
            string strategyName = $"{strategy.GetType().Name.PadRight(10)}  RsiLow {rsiLow} RsiHigh {rsiHigh} Stop {((double)stopLoss / 100).ToString("P0")}";
            List<StrategyResultsModel> strategyResults = new List<StrategyResultsModel>();
            List<PositionModel> totalPositions = new List<PositionModel>();

            Console.WriteLine($"Running Stratgy with RSI {rsiLow}/{rsiHigh} Stop {stopLoss.ToString("P0")}");

            foreach (string ticker in tickers)
            {
                SortedDictionary<DateTime, ConsolidatedStockModel> stockDictionary = stockDataLibrary[ticker];
                List<PositionModel> positions = strategy.RunBackTest(ticker, stockDictionary, from, to, rsiLow, rsiHigh, stopLoss);
                if (positions.Any())
                {
                    strategyResults.Add(BuildResults(strategyName, positions, ticker));

                    totalPositions.AddRange(positions);
                }
            }

            using (var writer = new StreamWriter($"{strategy.GetType().Name}-{rsiLow}-rsiHigh{rsiHigh}-Stop{stopLoss}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(totalPositions);
            }
            if (totalPositions.Any())
            {

                strategyResults.Add(BuildResults(strategyName, totalPositions, "SUMMARY"));
            }
            return strategyResults;
        }


        private static StrategyResultsModel BuildResults(string name, List<PositionModel> positions, string ticker)
        {
            double profit = positions.Sum(p => p.CloseValue - p.OpenValue);
            double wins = positions.Count(p => p.Win == true);
            double loose = positions.Count(p => p.Win == false);


            double daysOpen = positions.Average(p => (p.CloseDate - p.OpenDate).TotalDays);
            double maxDays = positions.Max(p => (p.CloseDate - p.OpenDate).TotalDays);
            double totalDays = positions.Sum(p => (p.CloseDate - p.OpenDate).TotalDays);
            int stops = positions.Count(p => p.Stop == true);
            return new StrategyResultsModel() { Ticker = ticker, Stops = stops, WinRate = wins / positions.Count, Profit = profit, MaxDays = Convert.ToInt32(maxDays), AverageDays = daysOpen, PositionsTaken = positions.Count(), Name = name, TotalDays = totalDays };
        }
        private static void PrintStrategyResults(StrategyResultsModel resultsModel)
        {
            Console.WriteLine($"{resultsModel.Ticker.PadRight(12)}{ resultsModel.Name.PadRight(60)}| {resultsModel.Profit.ToString("N2").PadRight(10)}{resultsModel.PositionsTaken.ToString().PadRight(15)}{ resultsModel.WinRate.ToString("P2").PadRight(10)}{resultsModel.AverageDays.ToString("N1").PadRight(12)}{resultsModel.Stops.ToString().PadRight(6)}{resultsModel.MaxDays.ToString().PadRight(8)}{resultsModel.TotalDays.ToString().PadRight(10)}");
        }

        private static void PrintStrategyHeader()
        {
            Console.WriteLine($"{"Market".PadRight(12)}{"Name".PadRight(60)}| {"Profit".PadRight(10)}{"PositionsTaken".PadRight(15)}{ "WinRate".PadRight(10)}{"AverageDays".PadRight(12)}{"Stops".ToString().PadRight(6)}{"MaxDays".ToString().PadRight(8)}{"TotalDays".PadRight(10)}");
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
            alphaKey = config["password"];
        }

        private static int Quote(QuoteOptions opts)
        {
            MarketSearch(opts.NamesToSearch.Trim(','), opts.Expiry, opts.EpicPrefix, opts.Detail);
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

            Console.WriteLine($"{"Name".PadRight(15)}{"Profit".PadRight(10)}\t{"Balance".PadRight(10)}\t{"Type".PadRight(14)}Account ID");
            foreach (Account account in accountModels.Accounts)
            {
                Console.WriteLine($"{account.AccountName.PadRight(15)} {account.Balance.ProfitLoss.ToString().PadRight(10)} \t{ Math.Round(account.Balance.BalanceBalance).ToString().PadRight(10)}\t{account.AccountType.PadRight(14)}{account.AccountId}");
            }
            return 0;
        }

        public static int Buy(BuyOptions options)
        {
            SwitchAccount(options.Account);
            List<Market> toBuy = MarketSearch(options.NamesToSearch.Trim(','), options.Expiry, options.EpicPrefix, false);

            int positionCount = toBuy.Count();

            float toInvest = (float)(options.Value / positionCount);

            List<IgBuy> buyList = new List<IgBuy>();
            int i = 0;

            Console.WriteLine($"Will buy");
            Console.WriteLine($"{"Name".PadRight(80)} {"Size".PadLeft(8)} {"Value".PadLeft(12)} {"Stop".PadLeft(10)} {"Limit".PadLeft(10)}");
            double total = 0;
            foreach (Market market in toBuy)
            {

                float positionSize = (float)Math.Round(toInvest / market.Bid, 2);
                if (positionSize < 0.5)
                {
                    positionSize = (float)0.5;
                }


                IgBuy igBuy = new IgBuy
                {
                    currencyCode = "GBP",
                    dealReference = $"NMCQ{i}",
                    direction = "BUY",
                    epic = market.Epic,
                    expiry = market.Expiry,
                    size = positionSize,
                    limitDistance = (float)(market.Bid * (options.LimitDistance / 100)),
                    stopDistance = (float)(market.Bid * (options.StopDistance / 100))
                };
                i++;

                buyList.Add(igBuy);

                total += positionSize * market.Bid;

                Console.WriteLine($"{market.InstrumentName.PadRight(80)} { positionSize.ToString().PadRight(8) }  { Math.Round(positionSize * market.Bid, 2).ToString("N0").PadLeft(12) }   { (market.Bid - igBuy.stopDistance).ToString("N2").PadLeft(10)} {(market.Bid + igBuy.limitDistance).ToString("N2").PadLeft(10)}");



            }


            Console.WriteLine($"Your total risk will be { total.ToString("N0") }");

            Console.WriteLine($"Do you wish to purchase?");
            var letter = Console.ReadKey();

            if (letter.Key == ConsoleKey.Y)
            {
                Console.WriteLine();

                Console.WriteLine($"Executing Order...");
                foreach (IgBuy buy in buyList)
                {
                    Buy(buy);
                }

            }

            return 0;
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

        public static List<Market> MarketSearch(string query, string expiry, string prefix, bool getDetail)
        {
            IgMarkets igMarkets = new IgMarkets(environment, login);
            string[] quotes = query.Split(',');

            int count = 0;
            List<Market> parsedList = new List<Market>();

            foreach (string ticker in quotes)
            {
                MarketSearchModel marketSearch = igMarkets.Get(session, ticker, getDetail);


                if (marketSearch != null && marketSearch.Markets != null)
                {
                    foreach (Market market in marketSearch.Markets)
                    {
                        if (market.Expiry == expiry && (string.IsNullOrEmpty(prefix) || market.Epic.StartsWith(prefix)))
                        {
                            parsedList.Add(market);
                            count++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Could not find '{ticker}'.");
                }
            }

            parsedList = parsedList.OrderBy(p => p.InstrumentName).ToList();
            parsedList.ForEach(market => Console.WriteLine($"{market.InstrumentName.PadLeft(70)} // {market.Epic.ToString().PadRight(30)} Bid {market.Bid}\tExpiry {market.Expiry} Ticker { (market.EpicModel?.instrument==null ? "No Chart Code" : market.EpicModel.instrument.chartCode) }"));


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
