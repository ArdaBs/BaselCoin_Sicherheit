using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NoSQLSkiServiceManager.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace NoSQLSkiServiceManager.Models
{
    public class ServiceOrder : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("customerName")]
        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; }

        [BsonElement("email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BsonElement("phoneNumber")]
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("desiredPickupDate")]
        public DateTime DesiredPickupDate { get; set; }

        [BsonElement("comments")]
        public string Comments { get; set; }

        [BsonElement("status")]
        [Required]
        public OrderStatus Status { get; set; }

        [BsonElement("serviceType")]
        [Required]
        public ServiceType ServiceType { get; set; }

        [BsonElement("priority")]
        [Required]
        public ServicePriority Priority { get; set; }
    }
}
