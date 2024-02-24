using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using NoSQLSkiServiceManager.Interfaces;

namespace NoSQLSkiServiceManager.Models
{
    public class Employee : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }

        [BsonElement("password")]
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [BsonElement("isLocked")]
        public bool IsLocked { get; set; }

        [BsonElement("failedLoginAttempts")]
        [Range(0, int.MaxValue)]
        public int FailedLoginAttempts { get; set; }

        [BsonElement("role")]
        [Required]
        public string Role { get; set; }
    }
}
