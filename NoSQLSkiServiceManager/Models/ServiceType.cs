using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace NoSQLSkiServiceManager.Models
{
    public class ServiceType
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public string Id { get; set; }

        [BsonElement("name")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [BsonElement("cost")]
        [Range(0.0, double.MaxValue)]
        public decimal Cost { get; set; }
    }
}
