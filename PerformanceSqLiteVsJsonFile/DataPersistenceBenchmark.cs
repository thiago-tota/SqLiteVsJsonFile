using BenchmarkDotNet.Attributes;
using DnsClient.Protocol;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Text.Json;

namespace PerformanceSqLiteVsJsonFile
{
    public class DataPersistenceBenchmark
    {
        private List<StockEntity> stocks;
        private string SearchFor = "CODE_90000";

        [Params(500000)]
        public int StockCount;

        [GlobalSetup]
        public void Setup()
        {
            stocks = GenerateRandomStocks(StockCount);
        }

        public void InitializeStocks(int stockCount)
        {
            stocks = GenerateRandomStocks(stockCount);
        }

        [Benchmark]
        public void PersistDataIntoSqLite()
        {
            using var dbContext = new SqLiteDataContext();
            dbContext.Database.EnsureCreated();
            dbContext.Stocks.AddRange(stocks);
            dbContext.SaveChanges();
        }

        [Benchmark]
        public StockEntity? SearchByCodeFromSqLite()
        {
            using var dbContext = new SqLiteDataContext();
            return dbContext.Stocks.FirstOrDefault(s => s.Code == SearchFor);
        }

        [Benchmark]
        public void PersistDataIntoJsonFile()
        {
            string json = JsonSerializer.Serialize(stocks);
            File.WriteAllText("DbStocks.json", json);
        }

        [Benchmark]
        public StockEntity? SearchByCodeFromJsonFile()
        {
            string json = File.ReadAllText("DbStocks.json");
            List<StockEntity> stocks = JsonSerializer.Deserialize<List<StockEntity>>(json);
            return stocks?.FirstOrDefault(s => s.Code == SearchFor);
        }

        [Benchmark]
        public StockEntity? SearchByCodeFromJsonFileStream()
        {
            var stocks = new List<StockEntity>();
            using FileStream fs = File.OpenRead("DbStocks.json");
            using JsonDocument document = JsonDocument.Parse(fs);
            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {
                string stockCode = element.GetProperty("Code").GetString()!;
                if (stockCode == SearchFor)
                {
                    stocks.Add(JsonSerializer.Deserialize<StockEntity>(element)!);
                }
            }

            return stocks.FirstOrDefault();
        }

        const string _mondoDbUrl = "mongodb://localhost:3017";
        const string _mongoDatabaseName = "MongoDbStocks";
        const string _mongoCollectionName = "StockCollection";

        [Benchmark]
        public void PersistDataIntoMongoDb()
        {
            var mongoClient = new MongoClient(_mondoDbUrl);
            var mongoDatabase = mongoClient.GetDatabase(_mongoDatabaseName);
            var mongoCollection = mongoDatabase.GetCollection<BsonDocument>(_mongoCollectionName);

            var bsonDocuments = stocks.Select(r => r.ToBsonDocument());
            mongoCollection.InsertMany(bsonDocuments);
        }

        [Benchmark]
        public StockEntity? SearchByCodeFromMongoDb()
        {
            var mongoClient = new MongoClient(_mondoDbUrl);
            var mongoDatabase = mongoClient.GetDatabase(_mongoDatabaseName);
            var mongoCollection = mongoDatabase.GetCollection<BsonDocument>(_mongoCollectionName);

            var filter = Builders<BsonDocument>.Filter.Eq("Code", SearchFor);
            var r = mongoCollection.Find(filter).First();
            return BsonSerializer.Deserialize<StockEntity>(r);
        }

        private List<StockEntity> GenerateRandomStocks(int count)
        {
            var random = new Random();
            var stocks = new List<StockEntity>();

            for (int i = 0; i < count; i++)
            {
                var stock = new StockEntity
                {
                    Id = Guid.NewGuid(),
                    Code = $"CODE_{i}",
                    Name = $"Stock_{i}",
                    BuyDate = DateTime.Now.AddDays(-random.Next(365)),
                    BuyQuantity = random.Next(100),
                    BuyPrice = (decimal)random.NextDouble() * 100,
                    LastQuote = (decimal)random.NextDouble() * 100,
                    SellDate = DateTime.Now.AddDays(-random.Next(365)),
                    SellQuantity = random.Next(100),
                    SellPrice = (decimal)random.NextDouble() * 100
                };

                stocks.Add(stock);
            }

            return stocks;
        }
    }
}
