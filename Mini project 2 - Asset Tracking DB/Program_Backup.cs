using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
// install-package Microsoft.EntityFrameworkCore -Version 5.0.13; install-package Microsoft.EntityFrameworkCore.SqlServer -Version 5.0.13; install-package Microsoft.EntityFrameworkCore.Tools -Version 5.0.13
// add-migration Init
// update-database
namespace Mini_project_2___Asset_Tracking_DB
{
    class Global
    {
        public static string Charset = "abcdefghijklmnopqrstuvwxyzåäöABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ0123456789_ "; //Not implemented yet
        public Global()
        { }
    }
    class Program
    {
        static void Main(string[] args)
        {
            AssetContext db = new AssetContext();
            DateTime dt1 = new DateTime(2022, 01, 20);
            DateTime dt2 = dt1.AddMonths(1);
            //Create (//No input from Console in this solution)
            PrepareOfficesIntoDb();
            PrepareAssetsIntoDb();
            PrepareExchangeRatesIntoDb();

            //Read
            ReadAssetsInDbAndPrint();

            db.AssetsSortingIdByType();
            ReadAssetsInDbAndPrint();
            db.AssetsSortingIdByModel();
            ReadAssetsInDbAndPrint();
            db.AssetsSortingIdByPurschaseDate();
            ReadAssetsInDbAndPrint();
            db.AssetsSortingIdByTypeThenByPurschaseDate();
            ReadAssetsInDbAndPrint();
            db.AssetsSortingIdByOfficeThenByDate();
            ReadAssetsInDbAndPrint();
            
            //Update


            //Delete
            DeleteAllAssetsOfSpecificBrand("Lenovo");
            DeleteAllAssetsOfSpecificModel("Razr");
            DeleteAssetsWithSpecificBrandAndModelBasedOnListOfAssets(PrepareListOfAssetsToBeRemoved());

            Console.WriteLine("Following assets are left in the db after removal.");
            ReadAssetsInDbAndPrint();
        }
        /// <summary>
        /// Creates a simulated list of inputs from user.
        /// </summary>
        /// <returns>List of Assets</returns>

        static void pressAnyKeyToContinue()
        {
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
        static void PrepareOfficesIntoDb()
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
        static void PrepareAssetsIntoDb()
        {
            Random r = new Random();
            AssetContext db = new AssetContext();
            List<Asset> assets = new List<Asset>()
            {
                new Computer(   "HP",      "Elitebook",    db.Offices.ToList().Find(o => o.Name == "Germany").Id,   GetDate("2019-06-01"), r.Next(1200, 1423),  "EUR",  10.12),
                new Computer(   "Asus",    "W234",         db.Offices.ToList().Find(o => o.Name == "USA").Id,       GetDate("2017-04-21"), r.Next(1000, 1223),  "USD",  1.00),
                new Computer(   "Lenovo",  "Yoga 530",     db.Offices.ToList().Find(o => o.Name == "USA").Id,       GetDate("2019-05-21"), r.Next(1000, 1100),  "USD",  1.00),
                new Computer(   "Lenovo",  "Yoga 730",     db.Offices.ToList().Find(o => o.Name == "USA").Id,       GetDate("2018-05-28"), r.Next(800, 900),    "USD",  1.00),
                new Computer(   "HP",      "Elitebook",    db.Offices.ToList().Find(o => o.Name == "Sweden").Id,    GetDate("2020-10-02"), r.Next(580, 620),    "SEK",  8.45),
                new Phone(      "iPhone",  "X",            db.Offices.ToList().Find(o => o.Name == "Sweden").Id,    GetDate("2018-07-15"), r.Next(1200, 1300),  "SEK",  8.45),
                new Phone(      "iPhone",  "11",           db.Offices.ToList().Find(o => o.Name == "Germany").Id,   GetDate("2020-09-25"), r.Next(900, 1100),   "EUR",  10.12),
                new Phone(      "iPhone",  "8",            db.Offices.ToList().Find(o => o.Name == "Germany").Id,   GetDate("2018-12-29"), r.Next(950, 1200),   "EUR",  10.12),
                new Phone(      "Motorola","Razr",         db.Offices.ToList().Find(o => o.Name == "Sweden").Id,    GetDate("2020-03-16"), r.Next(940, 1100),   "SEK",  8.45),
            };
            db.AddRange(assets);
            db.SaveChanges();
        }
        static void DeleteAllAssetsOfSpecificBrand(string brand)
        {
            List<Asset> aRemove = new List<Asset>();
            AssetContext db = new AssetContext();
            aRemove = db.Assets
                        .Where(a => a.Brand == brand)
                        .ToList();
            db.Assets.RemoveRange(aRemove);
            db.SaveChanges();
            Console.WriteLine("After all assets with the brand " + brand + " have been removed following are left in the db.");
            ReadAssetsInDbAndPrint();
        }
        static void DeleteAllAssetsOfSpecificModel(string model)
        {
            List<Asset> aRemove = new List<Asset>();
            AssetContext db = new AssetContext();
            aRemove = db.Assets
                        .Where(a => a.Model == model)
                        .ToList();
            db.Assets.RemoveRange(aRemove);
            db.SaveChanges();
            Console.WriteLine("After all assets with the model " + model + " have been removed following are left in the db.");
            ReadAssetsInDbAndPrint();
        }
        /// <summary>
        /// Removes assets from the database, based on brand and model of a list of objects.
        /// </summary>
        /// <returns>List of assets that was used when searching for assets to be removed in the DB</returns>
        static List<Asset> PrepareListOfAssetsToBeRemoved()
        {
            AssetContext db = new AssetContext();
            List<Asset> assetsToRemove = new List<Asset>()
            {
                new Computer(   "HP",      "Elitebook",    db.Offices.ToList().Find(o => o.Name == "Germany").Id,   GetDate("2019-06-01"), 1423,    "EUR",  10.12),
                new Computer(   "Asus",    "W234",         db.Offices.ToList().Find(o => o.Name == "USA").Id,       GetDate("2017-04-21"), 1100,    "USD",  1.00),
                new Phone(      "iPhone",  "X",            db.Offices.ToList().Find(o => o.Name == "Sweden").Id,    GetDate("2018-07-15"), 1245,    "SEK",  8.45),
                new Phone(      "iPhone",  "11",           db.Offices.ToList().Find(o => o.Name == "Germany").Id,   GetDate("2020-09-25"), 990,     "EUR",  10.12)
            };
            Console.WriteLine("Following list of assets has been prepared.");
            PrintDataFromListOfAssetsAndExchangeRates(db.Assets.ToList(), db.ExchangeRates.ToList());
            return assetsToRemove;
        }
        static void DeleteAssetsWithSpecificBrandAndModelBasedOnListOfAssets(List<Asset> assetsToRemoveFromDB)
        { 
            List<Asset> aRemove = new List<Asset>();
            AssetContext db = new AssetContext();
            foreach (Asset asset in assetsToRemoveFromDB)
            {
                aRemove = db.Assets
                            .Where(a => a.Brand == asset.Brand)
                            .Where(a => a.Model == asset.Model)
                            .ToList();
                db.Assets.RemoveRange(aRemove);
            }
            db.SaveChanges();
            Console.WriteLine("Following assets are to be used when searching the db to remove all records of assets with the same brand and model.");
            PrintHeader();
            PrintDataFromListOfAssetsAndExchangeRates(db.Assets.ToList(), db.ExchangeRates.ToList());
        }
        /// <summary>
        /// Creates a simulated list of exchange rates.
        /// </summary>
        /// <returns>List of Exchange rates</returns>
        static void PrepareExchangeRatesIntoDb()
        {
            List <ExchangeRate> exchangeRates = new List<ExchangeRate>()
            {
                new ExchangeRate("USD", 1.00),
                new ExchangeRate("SEK", 0.11),
                new ExchangeRate("EUR", 1.11),
                new ExchangeRate("DDK", 0.15)
            };
            AssetContext db = new AssetContext();
            foreach (ExchangeRate exchangeRate in exchangeRates)
            {
                //Update existing exchange rates of currecies already in the db
                db.ExchangeRates.Where(eR => eR.Currency == exchangeRate.Currency).ToList().ForEach(eR=>eR.Rate = exchangeRate.Rate);
                //Add currency if not in the db
                if (db.ExchangeRates.Where(eR => eR.Currency == exchangeRate.Currency).Count() == 0)
                {
                    db.Add(exchangeRate);
                }
            }
            //Remove duplicate entries in db.ExchangeRates (as in this article //https://www.c-sharpcorner.com/article/5-quick-ways-to-delete-duplicate-records-from-database-which-every-developer-mus/) for some reason I didn't understand the AsEnumerable() had to be added for it to work.
            var exchangeRateDupplicates = db.ExchangeRates.AsEnumerable().GroupBy(e => new { e.Currency, e.Rate }).SelectMany(grp => grp.Skip(1));
            db.ExchangeRates.RemoveRange(exchangeRateDupplicates);
            db.SaveChanges();
        }
        static void ReadAssetsInDbAndPrint()
        {
            Console.WriteLine("Assetrecords in database");
            PrintHeader();
            AssetContext db = new AssetContext();
            PrintDataFromListOfAssetsAndExchangeRates(db.Assets.ToList(), db.ExchangeRates.ToList());
            pressAnyKeyToContinue();
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
        static void PrintDataFromListOfAssetsAndExchangeRates(List<Asset> assets, List<ExchangeRate> exchangeRates)
        {
            assets.OrderBy(a => a.SortingId).ToList().ForEach(a => PreparePrintDataLine(a, exchangeRates));
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
        static void PreparePrintDataLine(Asset asset, List<ExchangeRate> exchangeRates) // This can be simplified by keeping exchange rates inside Assets and updating the rates at runtime.
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
                            Tab(db.Offices.ToList().FindLast(o => o.Id == asset.OfficeId).Name) +
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
            SortingId = Id;
            Brand = brand;
            Model = model;
            OfficeId = officeId;
            PurchaseDate = purchaseDate;
            PurchasePrice = purchasePrice;
            Currency = currency;
            ExchangeRate = exchangeRate;
        }
        public int Id { get; set; }
        public int SortingId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int OfficeId { get; set; }
        public string OfficeName { get; } 
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
        public Phone(string brand, string model, int officeId, DateTime purchaseDate, double purchasePrice, string currency, double exchangeRate) : base(brand, model, officeId, purchaseDate, purchasePrice, currency, exchangeRate)
        {
        }
    }
    class ExchangeRate
    {
        public int Id { get; set; }
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
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = MP2_AssetTrackingEF; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
        }
        public void AssetsSortingIdByType()
        {
            List<Asset> assets = Assets.AsEnumerable().OrderBy(a => a.GetType().Name).ToList();
            for (int i = 0; i < assets.Count; i++)
            {
                assets[i].SortingId = i;
            }
            SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green; 
            Console.WriteLine("SortingId of assets in db now ordered by type.");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void AssetsSortingIdByModel()
        {
            List<Asset> assets = Assets.AsEnumerable().OrderBy(a => a.Model).ToList();
            for (int i = 0; i < assets.Count; i++)
            {
                assets[i].SortingId = i;
            }
            SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SortingId of assets in db now ordered by model.");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void AssetsSortingIdByPurschaseDate()
        {
            List<Asset> assets = Assets.AsEnumerable().OrderBy(a => a.PurchaseDate).ToList();
            for (int i = 0; i < assets.Count; i++)
            {
                assets[i].SortingId = i;
            }
            SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SortingId of assets in db now ordered by purschase date.");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void AssetsSortingIdByTypeThenByPurschaseDate()
        {
            List<Asset> assets = Assets.AsEnumerable().OrderBy(a => a.GetType().Name).ThenBy(a => a.PurchaseDate).ToList();
            for (int i = 0;i < assets.Count;i++)
            {
                assets[i].SortingId = i;
            }
            SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SortingId of assets in db now ordered by type and then by purschase date.");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void AssetsSortingIdByOfficeThenByDate()
        {
            List<Asset> assets = Assets.AsEnumerable().OrderBy(a => a.Office).ThenBy(a => a.PurchaseDate).ToList();
            for (int i = 0; i < assets.Count; i++)
            {
                assets[i].SortingId = i;
            }
            SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SortingId of assets in db now ordered by office and then by purschase date.");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
