using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
// install-package Microsoft.EntityFrameworkCore -Version 5.0.13; install-package Microsoft.EntityFrameworkCore.SqlServer -Version 5.0.13; install-package Microsoft.EntityFrameworkCore.Tools -Version 5.0.13
// add-migration Init
// update-database
namespace Mini_project_2___Asset_Tracking_DB
{
    class Global
    {
        // Set allowed charset for all user string input
        public static string Charset = "abcdefghijklmnopqrstuvwxyzåäöABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ0123456789_ ";

        public Global()
        {
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            AssetContext db = new AssetContext();
            DateTime dt1 = new DateTime(2022, 01, 20);
            DateTime dt2 = dt1.AddMonths(1);
            //Create
            PrepareOfficesIntoDb();
            db.LoadECBCurrencyRatesFromLocalFileOrDownloadFromWebpageIntoDb();
            db.UpdateAssetsExchangeRatesInDb();

            UserMenuAssetDb();

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
            db.AssetsSortingIdByCountryThenByDate();
            ReadAssetsInDbAndPrint();
            
            //Update


            //Delete
            DeleteAllAssetsOfSpecificBrand("Lenovo");
            DeleteAllAssetsOfSpecificModel("Razr");

            Console.WriteLine("Following assets are left in the db after removal.");
            ReadAssetsInDbAndPrint();
        }
        static IEnumerable<Type> GetAllSubclassOf(Type parent)
        {
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var t in a.GetTypes())
                    if (t.IsSubclassOf(parent)) yield return t;
        }
        static void UserMenuAssetDb()
        {
            Console.WriteLine("This is the mini project 2 called Asset Tracking.");
            Console.WriteLine("It's a simple programming practice program where you create, process and display a list of items and recording them in an SQL-database.");
            Console.WriteLine("There is a class that automatically downloads and updates currency rates published by the European Central Bank and converts local purchase prices into USD.");
            AssetContext db = new AssetContext();
            List<Asset> newAssets = new List<Asset>();
            //Items items = new Items(new List<Item>());
            Console.WriteLine();
            Console.WriteLine("Press 1 to create a new list of assets, or ");
            Console.WriteLine("press 2 to use a prepared list of assets. ");
            Console.WriteLine("press ESC to quite the program.");
            ConsoleKeyInfo cky;
            bool asking = true;
            while (asking)
            {
                cky = Console.ReadKey(true);
                Console.WriteLine("You pressed: " + cky.KeyChar);
                Console.WriteLine();
                if (cky.Key == ConsoleKey.D1)
                {
                    AskNewListOfAssetsAndPrint(db);
                    break;
                }
                else if (cky.Key == ConsoleKey.Escape)
                { asking = false; break; }
                else if (cky.Key == ConsoleKey.D2)
                {
                    AddPreparedAssetsIntoDbAndPrint();
                    while (true)
                    {
                        Console.WriteLine("The prepared list is as follows:");
                        ReadAssetsInDbAndPrint();
                        Console.WriteLine("Press 1 to append new assets to this prepared list, or");
                        Console.WriteLine("Press 2 to use this prepared list as it is.");
                        cky = Console.ReadKey(true);
                        Console.WriteLine("You pressed: " + cky.KeyChar);
                        Console.WriteLine();
                        if (cky.Key == ConsoleKey.D1)
                        {
                            AskNewListOfAssetsAndPrint(db);
                            Console.WriteLine("Appending the new list to the prepared list of items.");
                            break;
                        }
                        else if (cky.Key == ConsoleKey.D2)
                        {
                            break;
                        }
                    }
                    break;
                }
            }
            Console.WriteLine("How do you want to order the list of assets? Press:");
            Console.WriteLine("1) By type");
            Console.WriteLine("2) By brand");
            Console.WriteLine("3) By model");
            Console.WriteLine("4) By country");
            Console.WriteLine("5) By purschase date");
            Console.WriteLine("6) By purschase price");
            Console.WriteLine("7) By type and then by purschase price");
            Console.WriteLine("8) By country of office and then by purschase date");
            Console.WriteLine("press ESC to quite the program.");
            while (true)
            {
                cky = Console.ReadKey(true);
                Console.WriteLine("You pressed: " + cky.KeyChar);
                Console.WriteLine();
                if (cky.Key == ConsoleKey.D1)
                {
                    db.AssetsSortingIdByType();
                    ReadAssetsInDbAndPrint();
                }
                if (cky.Key == ConsoleKey.D2)
                {
                    db.AssetsSortingIdByBrand();
                    ReadAssetsInDbAndPrint();
                }
                if (cky.Key == ConsoleKey.D3)
                {
                    db.AssetsSortingIdByModel();
                    ReadAssetsInDbAndPrint();
                }
                if (cky.Key == ConsoleKey.D4)
                {
                    db.AssetsSortingIdByCountry();
                    ReadAssetsInDbAndPrint();
                }
                if (cky.Key == ConsoleKey.D5)
                {
                    db.AssetsSortingIdByPurschaseDate();
                    ReadAssetsInDbAndPrint();
                }
                if (cky.Key == ConsoleKey.D6)
                {
                    db.AssetsSortingIdByPurschasePrice();
                    ReadAssetsInDbAndPrint();
                }
                if (cky.Key == ConsoleKey.D7)
                {
                    db.AssetsSortingIdByTypeThenByPurschaseDate();
                    ReadAssetsInDbAndPrint();
                }
  
                if (cky.Key == ConsoleKey.D8)
                {
                    db.AssetsSortingIdByCountryThenByDate();
                    ReadAssetsInDbAndPrint();
                }
                if (cky.Key == ConsoleKey.Escape)
                { 
                    break;
                    Console.WriteLine("THE END");
                }
            }
            List<Asset> AskNewListOfAssets()
            {
                List<Asset> newAssets = new List<Asset>();
                Console.WriteLine("Create a list of assets and write 'q' when finished.");
                string s;
                string type = null;
                string brand = null;
                string model = null;
                string country = null;
                int officeId = 0;
                string currencyISOCode = null;
                double purchasePrice = 0;
                string purchaseDate = null;
                bool isQuit = false;
                static bool CheckIsQuit(string s)
                {
                    if ((s == "q") | (s == null))
                    { return true; }
                    else
                    { return false; }
                }
                static bool GetCharsetCompliance(string s)
                {
                    if ((s == "") | (s == null))
                    { return false; }
                    foreach (char c in s)
                    {
                        if (!Global.Charset.Contains(c))
                        { return false; }
                    }
                    return true;
                }
                int i = 1;
                while (true)
                {
                    Console.WriteLine($"Create asset number {i}: ");
                    Console.Write($"   Select type: ");
                    while (true)
                    {
                        s = AskAssetType();
                        if (CheckIsQuit(s))
                        { isQuit = true; break; }
                        if (GetCharsetCompliance(s))
                        { type = s; break; }
                        else { AlertAllowedCharset(); }
                    }
                    if (isQuit) { break; }
                    Console.Write($"   Enter brand: ");
                    while (true)
                    {
                        s = Console.ReadLine();
                        if (CheckIsQuit(s))
                        { isQuit = true; break; }
                        if (GetCharsetCompliance(s))
                        { brand = s; break; }
                        else { AlertAllowedCharset(); }
                    }
                    if (isQuit) { break; }

                    Console.Write("   Enter model: ");
                    while (true)
                    {
                        s = Console.ReadLine();
                        if (CheckIsQuit(s))
                        { isQuit = true; break; }
                        if (GetCharsetCompliance(s))
                        { model = s; break; }
                        else { AlertAllowedCharset(); }
                    }
                    if (isQuit) { break; }

                    Console.Write("   Select country of office location from list by entering a number:");
                    AssetContext db = new AssetContext();
                    while (true)
                    {
                        foreach (Office office in db.Offices)
                        {
                            Console.WriteLine("     " + office.Id + " : " + office.Country + " using the currency " + office.Currency);
                        }
                        s = Console.ReadLine();
                        if (Int32.TryParse(s, out int iChoice))
                        {
                            if ((iChoice >= 0) && (iChoice < db.Offices.Count()))
                            {
                                officeId = iChoice;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Please enter a valid number to select country of office location:");
                        }
                        if (CheckIsQuit(s))
                        { isQuit = true; break; }
                        if (GetCharsetCompliance(s))
                        { country = s; break; }
                        else { AlertAllowedCharset(); }
                    }

                    Console.Write("   Enter productprice: ");
                    while (true)
                    {
                        s = Console.ReadLine();
                        bool isDouble = double.TryParse(s, out double d);
                        if (CheckIsQuit(s))
                        { isQuit = true; break; }
                        else if (isDouble)
                        { purchasePrice = d; break; }
                        else
                        {
                            Console.WriteLine("Input is not a number, please try again to enter a productprice: ");
                        }
                    }
                    Console.Write("   Enter purchase date ('yyyy-MM-dd'): ");
                    while (true)
                    {
                        s = Console.ReadLine();
                        bool isDateTimeParseExact = DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeInput);
                        if (CheckIsQuit(s))
                        { isQuit = true; break; }
                        else if (isDateTimeParseExact)
                        { purchaseDate = dateTimeInput.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); break; }
                        else
                        {
                            Console.WriteLine("Input is not formatted as a date, please use the date format 'yyyy-MM-dd': ");
                        }
                    }
                    if (isQuit) { break; }
                    switch (type) 
                    {
                        case "Phone":
                            newAssets.Add(
                                new Phone(
                                    brand,
                                    model,
                                    officeId,
                                    GetDate(purchaseDate),
                                    purchasePrice
                                    )); break;
                        case "Computer":
                            newAssets.Add(
                                new Computer(
                                    brand,
                                    model,
                                    officeId,
                                    GetDate(purchaseDate),
                                    purchasePrice
                                    )); break;
                    }
                    i++;
                }
                return newAssets;
                static void AlertAllowedCharset()
                {
                    Console.WriteLine("Entered text is using non allowed characters.");
                    Console.WriteLine("Please try again and use only the following characters:");
                    Console.WriteLine(Global.Charset);
                }
                string AskAssetType()
                {
                    Asset asset = new Asset("HP", "Some model", 1, GetDate("2011-12-10"), 234); //Dummy asset
                    IEnumerable<Type> assetTypes = GetAllSubclassOf(asset.GetType());
                    string s;
                    Console.WriteLine("Enter number to select asset type:");
                    int iMax = assetTypes.Count();
                    int iChoice = -1;
                    while (true)
                    {
                        for (int i = 0; i < iMax; i++)
                        {
                            Console.WriteLine(i + ") type: " + assetTypes.ElementAt(i).Name);
                        }
                        s = Console.ReadLine();
                        Int32.TryParse(s, out iChoice);
                        if ((iChoice >= 0) && (iChoice <= iMax))
                            break;        
                    }
                    return assetTypes.ElementAt(iChoice).Name;
                }
            }
            void AskNewListOfAssetsAndPrint(AssetContext db)
            {
                List<Asset> preparedListOfAssets = AskNewListOfAssets();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("These assets have been entered by the user and will be added to the database.");
                Console.ResetColor();
                PrintHeader();
                PrintDataFromListOfAssetsAndExchangeRates(preparedListOfAssets, db.ExchangeRates.ToList());
                db.Assets.AddRange(preparedListOfAssets);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Now these assets are recorded in the db.");
                Console.ResetColor();
                ReadAssetsInDbAndPrint();
            }
        }
        static void pressAnyKeyToContinue()
        {
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
        static void PrepareOfficesIntoDb()
        {
        List<Office> offices = new List<Office>()
            {
                new Office("HQ", "Sweden","SEK"),
                new Office("Sales office","USA", "USD"),
                new Office("Sales office","Germany","EUR"),
                new Office("Sales office","Denmark","DDK")
            };
            AssetContext db = new AssetContext();
            foreach (Office office in offices)
            {
                if (db.Offices.ToList().Where(o => o.Name == office.Name)
                                        .Where(o => o.Country == office.Country)
                                        .Where(o => o.Currency == office.Currency)
                                        .Count() == 0)
                {
                    db.Offices.Add(office);
                }
            }
            db.SaveChanges();
        }
        /// <summary>
        /// Creates a simulated list of assets and enters them into the database.
        /// </summary>
        /// <returns></returns>
        static List<Asset> AddPreparedAssetsIntoDbAndPrint()
        {
            Random r = new Random();
            AssetContext db = new AssetContext();
            List<Asset> preparedListOfAssets = new List<Asset>()
            {
                new Computer(   "HP",       "Elitebook",    3,  GetDate("2019-06-01"), r.Next(1200, 1423)),
                new Computer(   "Asus",     "W234",         2,  GetDate("2017-04-21"), r.Next(1000, 1223)),
                new Computer(   "Lenovo",   "Yoga 530",     2,  GetDate("2019-05-21"), r.Next(1000, 1100)),
                new Computer(   "Lenovo",   "Yoga 730",     2,  GetDate("2018-05-28"), r.Next(800, 900)),
                new Computer(   "HP",       "Elitebook",    1,  GetDate("2020-10-02"), r.Next(5800, 6200)),
                new Phone(      "iPhone",   "X",            1,  GetDate("2018-07-15"), r.Next(12000, 13000)),
                new Phone(      "iPhone",   "11",           3,  GetDate("2020-09-25"), r.Next(900, 1100)),
                new Phone(      "iPhone",   "8",            3,  GetDate("2018-12-29"), r.Next(950, 1200)),
                new Phone(      "Motorola", "Razr",         1,  GetDate("2020-03-16"), r.Next(9400, 11000)),
                new Phone(      "Samsung",  "A72",          1,  GetDate("2021-09-14"), r.Next(5190, 5300)),
                new Computer(   "ASUS",     "N56JV",        1,  GetDate("2013-02-02"), r.Next(13000,15000)),
                new Phone(      "Samsung",  "S7",           1,  GetDate("2019-05-01"), r.Next(6000, 8000)),
                new Phone(      "HTC",      "Desire Z",     3,  GetDate("2011-06-01"), r.Next(400, 550))
            };
            foreach (Asset asset in preparedListOfAssets)
            {
                asset.Currency = db.Offices.Where(o => o.Id == asset.OfficeId).First().Currency;
                asset.ExchangeRate = db.ExchangeRates.Where(o => o.Currency == db.Offices.Where(o => o.Id == asset.OfficeId).First().Currency).First().Rate; //This is really too complicated. There just must be a more elegant and simpler way of doing this.
                asset.ExchangeRateUSD = db.ExchangeRates.Where(o => o.Currency == "USD").First().Rate;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("These assets have been prepared and will be added to the database.");
            Console.ResetColor();
            PrintHeader();
            PrintDataFromListOfAssetsAndExchangeRates(preparedListOfAssets, db.ExchangeRates.ToList());
            db.AddRange(preparedListOfAssets);
            db.SaveChanges();
            return preparedListOfAssets;
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
        public Asset(string brand, string model, int officeId, DateTime purchaseDate, double purchasePrice)
        {
            Brand = brand;
            Model = model;
            OfficeId = officeId;
            PurchaseDate = purchaseDate;
            PurchasePrice = purchasePrice;
        }
        public int Id { get; set; }
        public int SortingId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int OfficeId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public double PurchasePrice { get; set; }
        public string Currency { get; set; }
        public double ExchangeRate { get; set; }
        public double ExchangeRateUSD { get; set; }
    }
    class Computer : Asset
    {
        public Computer(string brand, string model, int officeId, DateTime purchaseDate, double purchasePrice) : base(brand, model, officeId, purchaseDate, purchasePrice)
        {
        }
    }
    class Phone : Asset
    {
        public Phone(string brand, string model, int officeId, DateTime purchaseDate, double purchasePrice) : base(brand, model, officeId, purchaseDate, purchasePrice)
        {
        }
    }

    class Office
    {
        public int Id { get; set; }
        public Office(string name, string country, string currency)
        {
            Name = name;
            Country = country;
            Currency = currency;
        }
        public Office(string country)
        {
            Country = country;
        }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
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
        const string URLXML = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"; //@"c:\Users\Christian\source\VS\Csharp\Mini project 1 - Asset Tracking\eurofxref-daily.xml";
        public static string localTempPath = Path.GetTempPath();
        public string localTempPathFileName = (@"" + localTempPath + "eurofxref-daily.xml");
        public void LoadECBCurrencyRatesFromLocalFileOrDownloadFromWebpageIntoDb()
        {
            //https://stackoverflow.com/questions/17250660/how-to-parse-xml-file-from-european-central-bank-with-python
            if (!File.Exists(localTempPathFileName))
            {
                Console.WriteLine("Currency table not found on local disc, thus downloading it.");
                DownloadECBCurrencyRatesFromWebpageToLocalDisc();
            }
            // Check date inside of localy saved eurofxref-daily.xml
            XmlReader reader = XmlReader.Create(localTempPathFileName);
            XElement root = XElement.Load(reader);
            XmlNameTable nameTable = reader.NameTable;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace("ns", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
            IEnumerable<XElement> dateAndCurrencyConverionsList = root.XPathSelectElements("./ns:Cube/ns:Cube[@time]", namespaceManager);
            string dateLocalSavedCurrencyConversionXML = dateAndCurrencyConverionsList.ElementAt(0).Attribute("time").Value;
            DateTime.TryParseExact(dateLocalSavedCurrencyConversionXML, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateLocalSavedCurrencyConversionXMLAsDateTime);
            TimeSpan AgeLocalXLM = DateTime.Now - dateLocalSavedCurrencyConversionXMLAsDateTime;
            Console.WriteLine("Currency table on local disc contains the date " + dateLocalSavedCurrencyConversionXML + ".");
            if (AgeLocalXLM.Days > 1)
            {
                reader.Close();
                Console.WriteLine("Currency table found on local disc, but more than 1 day old, thus redownloading it.");
                DownloadECBCurrencyRatesFromWebpageToLocalDisc();
                XmlReader reader2 = XmlReader.Create(localTempPathFileName);
                XElement root2 = XElement.Load(reader2);
                root = root2;
            }
            IEnumerable<XElement> currecyConversionList = root.XPathSelectElements("./ns:Cube/ns:Cube/ns:Cube[@currency]", namespaceManager);
            int elementsCount = currecyConversionList.Count();
            List<ExchangeRate> exchangeRates = new List<ExchangeRate>()
                { new ExchangeRate("EUR", 1)}; // The list is from ECB and is a record of exchange rates in relation to EUR, so EUR is not in the list.
            AssetContext db = new AssetContext();
            foreach (XElement el in currecyConversionList)
            {
                //Console.WriteLine(el);
                //Console.WriteLine(el.Attribute("currency").Value);
                //Console.WriteLine(el.Attribute("rate").Value);
                exchangeRates.Add(
                    new ExchangeRate(
                        el.Attribute("currency").Value,
                        double.Parse(el.Attribute("rate").Value, CultureInfo.InvariantCulture)));
            }
            foreach (ExchangeRate exchangeRate in exchangeRates)
            {
                //Update existing exchange rates of currecies already in the db
                db.ExchangeRates.Where(eR => eR.Currency == exchangeRate.Currency).ToList().ForEach(eR => eR.Rate = exchangeRate.Rate);
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
        public bool DownloadECBCurrencyRatesFromWebpageToLocalDisc()
        {
            System.Net.WebClient webClient = new WebClient();
            try
            {
                //File.Delete(tempPathFileName);
                webClient.DownloadFile(URLXML, localTempPathFileName);
                webClient.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Download of " + URLXML + " failed.");
            }
            Console.WriteLine("Currency table downloaded from: " + URLXML);
            return true;
        }
        public void UpdateAssetsExchangeRatesInDb()
        {
            AssetContext db = new AssetContext();
            foreach (Asset asset in Assets)
            {
                asset.ExchangeRate = db.ExchangeRates.Where(c => c.Currency == asset.Currency).FirstOrDefault().Rate;
                asset.ExchangeRateUSD = db.ExchangeRates.Where(c => c.Currency == "USD").FirstOrDefault().Rate;
            }
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
        public void AssetsSortingIdByBrand()
        {
            List<Asset> assets = Assets.AsEnumerable().OrderBy(a => a.Brand).ToList();
            for (int i = 0; i < assets.Count; i++)
            {
                assets[i].SortingId = i;
            }
            SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SortingId of assets in db now ordered by brand.");
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
        public void AssetsSortingIdByCountry()
        {
            List<Asset> assets = Assets.AsEnumerable().OrderBy(a => a.OfficeId).ToList();
            for (int i = 0; i < assets.Count; i++)
            {
                assets[i].SortingId = i;
            }
            SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SortingId of assets in db now ordered by country of office.");
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
        public void AssetsSortingIdByPurschasePrice()
        {
            List<Asset> assets = Assets.AsEnumerable().OrderBy(a => a.PurchasePrice).ToList();
            for (int i = 0; i < assets.Count; i++)
            {
                assets[i].SortingId = i;
            }
            SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SortingId of assets in db now ordered by purschase price.");
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
        public void AssetsSortingIdByCountryThenByDate()
        {
            List<Asset> assets = Assets.AsEnumerable().OrderBy(a => a.OfficeId).ThenBy(a => a.PurchaseDate).ToList();
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
