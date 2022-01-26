using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mini_project_2___Asset_Tracking_DB
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt1 = new DateTime(2022, 01, 20);
            DateTime dt2 = dt1.AddMonths(1);
            //No input from Console in this solution
            List<Asset> assets = PrepareAssets();
            List<ExchangeRate> exchangeRates = PrepareExchangeRates();
            //assets = SortAssetsByTypeAndDate(assets);
            assets = SortAssetsByOfficeAndDate(assets);
            PrintHeader();
            PrintData(assets, exchangeRates);

            Console.ReadLine();
        }
        /// <summary>
        /// Creates a simulated list of inputs from user.
        /// </summary>
        /// <returns>List of Assets</returns>
        static List<Asset> PrepareAssets()
        {
            Office officeSweden = new Office("Sweden");
            Office officeUsa = new Office("USA");
            Office officeSpain = new Office("Spain");
            //var officeSpain = new Office("Spain"); //Other way to create an Office object
            //Office officeSpain = new("Spain");     //Other way to create an Office object
            return new List<Asset>()
            {
                new Computer("HP", "Elitebook", GetDate("2019-06-01"), officeSpain, 1423, "EUR",10.12),
                new Phone("iPhone", "X", GetDate("2018-07-15"),officeSweden, 1245, "SEK",8.45),
                new Computer("Asus", "W234", GetDate("2017-04-21"),officeUsa, 1200, "USD",1.00),
                new Phone("iPhone", "11", GetDate("2020-09-25"),officeSpain, 990, "EUR",10.12),
                new Computer("Lenovo", "Yoga 530", GetDate("2019-05-21"),officeUsa, 1030, "USD",1.00),
                new Phone("iPhone", "8", GetDate("2018-12-29"),officeSpain, 970, "EUR",10.12),
                new Computer("Lenovo", "Yoga 730", GetDate("2018-05-28"),officeUsa, 835, "USD",1.00),
                new Phone("Motorola", "Razr", GetDate("2020-03-16"),officeSweden, 970, "SEK",8.45),
                new Computer("HP", "Elitebook", GetDate("2020-10-02"),officeSweden, 588, "SEK",8.45)
            };
        }
        /// <summary>
        /// Creates a simulated list of exchange rates.
        /// </summary>
        /// <returns>List of Exchange rates</returns>
        static List<ExchangeRate> PrepareExchangeRates()
        {
            return new List<ExchangeRate>()
            {
                new ExchangeRate("USD",1.00),
                new ExchangeRate("SEK", 0.12),
                new ExchangeRate("EUR", 1.21)
            };
        }
        /// <summary>
        /// Sorts the Asset list by asset type and purchase date.
        /// </summary>
        /// <param name="assets"></param>
        /// <returns>List of sorted Assets</returns>
        static List<Asset> SortAssetsByTypeAndDate(List<Asset> assets)
        {
            assets = assets.OrderBy(asset => asset.GetType().Name).ThenBy(asset => asset.PurchaseDate).ToList();
            return assets;
        }
        /// <summary>
        /// Sorts the Asset list by office and purchase date.
        /// </summary>
        /// <param name="assets"></param>
        /// <returns>List of sorted Assets</returns>
        static List<Asset> SortAssetsByOfficeAndDate(List<Asset> assets)
        {
            assets = assets.OrderBy(asset => asset.Office.Name).ThenBy(asset => asset.PurchaseDate).ToList();
            return assets;
        }
        /// <summary>
        /// Prints the Header columns to Console.
        /// </summary>
        static void PrintHeader()
        {
            Console.WriteLine(
                Tab("Type") +
                Tab("Brand") +
                Tab("Model") +
                Tab("Office") +
                Tab("Purchase Date") +
                Tab("Price in USD") +
                Tab("Currency") +
                Tab("Local price today")
                );
            Console.WriteLine(
                Tab("----") +
                Tab("-----") +
                Tab("-----") +
                Tab("------") +
                Tab("-------------") +
                Tab("------------") +
                Tab("--------") +
                Tab("-----------------")
                );
        }
        /// <summary>
        /// Prints data.
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="exchangeRates"></param>
        static void PrintData(List<Asset> assets, List<ExchangeRate> exchangeRates)
        {
            assets.ForEach(asset => PreparePrintDataLine(asset, exchangeRates));
        }
        /// <summary>
        /// Converts date from string to DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns>DateTime</returns>
        static DateTime GetDate(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Prepares data to be printed.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="exchangeRates"></param>
        static void PreparePrintDataLine(Asset asset, List<ExchangeRate> exchangeRates)
        {
            int daysWarning = 913; //Approx 30 months
            int daysAlarm = 1004;  //Approx 33 months
            TimeSpan diff = DateTime.Now - asset.PurchaseDate;
            DecideForegroundColor(daysWarning, daysAlarm, diff);

            double usdRateToday = exchangeRates
                .Where(exchangeRate => exchangeRate.Currency == asset.Currency)
                .Select(ex => ex.Rate)
                .FirstOrDefault();
            PrintDataLine(asset, usdRateToday);

            Console.ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// Decides what Console Foreground Color to be used 
        /// depending on how old assets are.
        /// </summary>
        /// <param name="daysWarning"></param>
        /// <param name="daysAlarm"></param>
        /// <param name="diff"></param>
        static void DecideForegroundColor(int daysWarning, int daysAlarm, TimeSpan diff)
        {
            if (diff.Days > daysAlarm)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (diff.Days > daysWarning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        /// <summary>
        /// Prints data for one asset to Console.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="usdRateToday"></param>
        static void PrintDataLine(Asset asset, double usdRateToday)
        {
            Console.WriteLine(
                            Tab(asset.GetType().Name) +
                            Tab(asset.Brand) +
                            Tab(asset.Model) +
                            Tab(asset.Office.Name) +
                            Tab(asset.PurchaseDate.ToShortDateString()) +
                            Tab(asset.PurchasePrice.ToString("0.##")) +
                            Tab(asset.Currency) +
                            Tab((asset.PurchasePrice / usdRateToday).ToString("0.##"))
                            );
        }
        /// <summary>
        /// PadRight with 14 characters on a string. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string Tab(string input)
        {
            return input.PadRight(14);
        }
        static string TabLeft(string input)
        {
            return input.PadLeft(10);
        }
    }
    class Asset
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public DateTime PurchaseDate { get; set; }
        public Office Office { get; set; }
        public double PurchasePrice { get; set; }
        public string Currency { get; set; }
        public double ExchangeRate { get; set; }
    }
    class Computer : Asset
    {
        public Computer(string brand, string model, DateTime purchaseDate, Office office, double purchasePrice, string currency, double exchangeRate)
        {
            Brand = brand;
            Model = model;
            PurchaseDate = purchaseDate;
            Office = office;
            PurchasePrice = purchasePrice;
            Currency = currency;
            ExchangeRate = exchangeRate;
        }
    }
    class Phone : Asset
    {
        public Phone(string brand, string model, DateTime purchaseDate, Office office, double purchasePrice, string currency, double exchangeRate)
        {
            Brand = brand;
            Model = model;
            PurchaseDate = purchaseDate;
            Office = office;
            PurchasePrice = purchasePrice;
            Currency = currency;
            ExchangeRate = exchangeRate;
        }
    }
    class ExchangeRate
    {
        public ExchangeRate(string currency, double rate)
        {
            Currency = currency;
            Rate = rate;
        }
        public string Currency { get; set; }
        public double Rate { get; set; }
    }
    class Office
    {
        public Office(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
    class VehicleContext : DbContext
    {
        public DbSet<Asset> Assets { get; set; }
        //public DbSet<Compuer> Computers { get; set; }
        //public DbSet<Phone> Phones { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = MP2_AssetTrackingEF; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
        }
    }
}
