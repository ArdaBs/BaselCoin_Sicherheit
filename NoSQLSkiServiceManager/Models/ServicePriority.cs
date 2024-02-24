using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace NoSQLSkiServiceManager.Models
{
    public class ServicePriority
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public string Id { get; set; }

        [BsonElement("priorityName")]
        [Required]
        [StringLength(100)]
        public string PriorityName { get; set; }

        [BsonElement("dayCount")]
        [Range(0, int.MaxValue)]
        public int DayCount { get; set; }
    }
}
