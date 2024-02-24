using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using NoSQLSkiServiceManager.Models;

namespace NoSQLSkiServiceManager.DTOs.Response
{
    public class ServiceOrderAggregateDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CustomerName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public ServicePriority Priority { get; set; }

        public ServiceType ServiceTypeId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime DesiredPickupDate { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }

        public IEnumerable<ServiceType> ServiceTypes { get; set; }
        public IEnumerable<ServicePriority> ServicePriorities { get; set; }
    }

}
