using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace PerformanceSqLiteVsJsonFile
{
    public class StockEntity
    {
        [Key, BsonId]
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime BuyDate { get; set; }
        public int BuyQuantity { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal LastQuote { get; set; }
        public DateTime? SellDate { get; set; }
        public int? SellQuantity { get; set; }
        public decimal? SellPrice { get; set; }
    }
}
