using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
// install-package Microsoft.EntityFrameworkCore -Version 5.0.13; install-package Microsoft.EntityFrameworkCore.SqlServer -Version 5.0.13; install-package Microsoft.EntityFrameworkCore.Tools -Version 5.0.13
// add-migration Init
// update-database
namespace Mini_project_2___Asset_Tracking_DB
{
    class Program
    {
        static void Main(string[] args)
        {
            AssetContext db = new AssetContext();
            DateTime dt1 = new DateTime(2022, 01, 20);
            DateTime dt2 = dt1.AddMonths(1);
            //Create (//No input from Console in this solution)
            List<Office> offices = PrepareOffices();
            List<Asset> assets = PrepareAssets();
            List<ExchangeRate> exchangeRates = PrepareExchangeRates();
            //assets = SortAssetsByTypeAndDate(assets);
            //assets = SortAssetsByOfficeAndDate(assets);
            Console.WriteLine("Following list of assets is prepared to be added to the DB.");
            PrintHeader();
            PrintData(assets, exchangeRates);
            Console.ReadLine();
            Console.WriteLine("Following assets are recorded in the DB.");
            PrintHeader();
            PrintData(ReadAssetsFromDB(), exchangeRates);
            Console.ReadLine();
            //Delete
            Console.WriteLine("Following assets are to be used when searching the db to remove all records of assets with the same brand and model.");
            List<Asset> assetsToRemove = DeleteAssetsBasedOnBrandAndModel();
            PrintHeader();
            PrintData(assetsToRemove, exchangeRates);
            Console.ReadLine();
            Console.WriteLine("Following assets are left in the db after removal.");
            PrintHeader(); 
            PrintData(ReadAssetsFromDB(), exchangeRates);
        }
        /// <summary>
        /// Creates a simulated list of inputs from user.
        /// </summary>
        /// <returns>List of Assets</returns>
        static List<Office> PrepareOffices()
        {
            List<Office> offices = new List<Office>()
            {
                new Office("Sweden"),
                new Office("USA"),
                new Office("Germany"),
                new Office("Denmark")
            };
            AssetContext db = new AssetContext();
            foreach (Office office in offices)
            {
                if (db.Offices.ToList().Where(o => o.Name == office.Name).Count() == 0)
                {
                    db.Offices.Add(office);
                }
            }
            db.SaveChanges();
            return offices;
        }
        /// <summary>
        /// Reads all assets from DB
        /// </summary>
        /// <returns>List of read assets</returns>
        static List<Asset> ReadAssetsFromDB()
        {
            AssetContext db = new AssetContext();
            return db.Assets.ToList();
        }
            /// <summary>
            /// Creates a simulated list of assets and enters them into the database.
            /// </summary>
            /// <returns>List of Exchange rates</returns>
            static List<Asset> PrepareAssets()
        {
            AssetContext db = new AssetContext();
            List<Asset> assets = new List<Asset>()
            {
                new Computer(   "HP",      "Elitebook",    db.Offices.ToList().Find(o => o.Name == "Germany").Id,   GetDate("2019-06-01"), 1423,    "EUR",  10.12),
                new Computer(   "Asus",    "W234",         db.Offices.ToList().Find(o => o.Name == "USA").Id,       GetDate("2017-04-21"), 1100,    "USD",  1.00),
                new Computer(   "Lenovo",  "Yoga 530",     db.Offices.ToList().Find(o => o.Name == "USA").Id,       GetDate("2019-05-21"), 1030,    "USD",  1.00),
                new Computer(   "Lenovo",  "Yoga 730",     db.Offices.ToList().Find(o => o.Name == "USA").Id,       GetDate("2018-05-28"), 835,     "USD",  1.00),
                new Computer(   "HP",      "Elitebook",    db.Offices.ToList().Find(o => o.Name == "Sweden").Id,    GetDate("2020-10-02"), 588,     "SEK",  8.45),
                new Phone(      "iPhone",  "X",            db.Offices.ToList().Find(o => o.Name == "Sweden").Id,    GetDate("2018-07-15"), 1245,    "SEK",  8.45),
                new Phone(      "iPhone",  "11",           db.Offices.ToList().Find(o => o.Name == "Germany").Id,   GetDate("2020-09-25"), 990,     "EUR",  10.12),
                new Phone(      "iPhone",  "8",            db.Offices.ToList().Find(o => o.Name == "Germany").Id,   GetDate("2018-12-29"), 970,     "EUR",  10.12),
                new Phone(      "Motorola","Razr",         db.Offices.ToList().Find(o => o.Name == "Sweden").Id,    GetDate("2020-03-16"), 970,     "SEK",  8.45),
            };
            /*            foreach (Asset asset in assets)  
                        {
                            if (!db.Assets.ToList().Equals(asset))  // This comparison doesn't really work, probably because they have different Office.Id.
                            {
                                db.Assets.Add(asset);
                            }
                        }*/
            db.AddRange(assets);
            db.SaveChanges();
            return assets;
        }
        /// <summary>
        /// Removes assets from the database, based on brand and model of a list of objects.
        /// </summary>
        /// <returns>List of assets that was used when searching for assets to be removed in the DB</returns>
        static List<Asset> DeleteAssetsBasedOnBrandAndModel()
        {
            AssetContext db = new AssetContext();
            List<Asset> assetsToRemove = new List<Asset>()
            {
                new Computer(   "HP",      "Elitebook",    db.Offices.ToList().Find(o => o.Name == "Germany").Id,   GetDate("2019-06-01"), 1423,    "EUR",  10.12),
                new Computer(   "Asus",    "W234",         db.Offices.ToList().Find(o => o.Name == "USA").Id,       GetDate("2017-04-21"), 1100,    "USD",  1.00),
                new Phone(      "iPhone",  "X",            db.Offices.ToList().Find(o => o.Name == "Sweden").Id,    GetDate("2018-07-15"), 1245,    "SEK",  8.45),
                new Phone(      "iPhone",  "11",           db.Offices.ToList().Find(o => o.Name == "Germany").Id,   GetDate("2020-09-25"), 990,     "EUR",  10.12)
            };
            List<Asset> assetsToRemoveDB = new List<Asset>();
            foreach (Asset asset in assetsToRemove)
            {
                assetsToRemoveDB = db.Assets
                                        .Where(a => a.Brand == asset.Brand)
                                        .Where(a => a.Model == asset.Model)
                                        .ToList();
                db.Assets.RemoveRange(assetsToRemoveDB);
            }
            db.SaveChanges();
            return assetsToRemove;
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
/*        static List<Asset> SortAssetsByOfficeAndDate(List<Asset> assets)
        {
            assets = assets.OrderBy(asset => asset.Office.Name).ThenBy(asset => asset.PurchaseDate).ToList();
            return assets;
        }*/
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
            AssetContext db = new AssetContext();
            Console.WriteLine(
                            Tab(asset.GetType().Name) +
                            Tab(asset.Brand) +
                            Tab(asset.Model) +
                            Tab(db.Offices.ToList().FindLast(o=> o.Id == asset.OfficeId).Name) +
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
        public Asset(string brand, string model, int officeId, DateTime purchaseDate, double purchasePrice, string currency, double exchangeRate)
        {
            Brand = brand;
            Model = model;
            OfficeId = officeId;
            PurchaseDate = purchaseDate;
            PurchasePrice = purchasePrice;
            Currency = currency;
            ExchangeRate = exchangeRate;
        }
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int OfficeId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public Office Office { get; set; }
        public double PurchasePrice { get; set; }
        public string Currency { get; set; }
        public double ExchangeRate { get; set; }
    }
    class Computer : Asset
    {
        public Computer(string brand, string model, int officeId, DateTime purchaseDate, double purchasePrice, string currency, double exchangeRate) : base(brand, model, officeId, purchaseDate, purchasePrice, currency, exchangeRate)
        {
        }
    }
    class Phone : Asset
    {
        public Phone(string brand, string model, int officeId, DateTime purchaseDate, double purchasePrice, string currency, double exchangeRate) : base (brand, model, officeId, purchaseDate, purchasePrice, currency, exchangeRate)
        {
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
        public int Id { get; set; }
        public Office(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
    class AssetContext : DbContext
    {
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Office> Offices { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = MP2_AssetTrackingEF; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
        }
    }
}
