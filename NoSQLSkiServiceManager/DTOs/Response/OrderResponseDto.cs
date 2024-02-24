using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using NoSQLSkiServiceManager.Interfaces;
using NoSQLSkiServiceManager.Models;

namespace NoSQLSkiServiceManager.DTOs.Response
{
    public class OrderResponseDto : IResponseDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CustomerName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public ServiceTypeDto ServiceType { get; set; }
        public ServicePriorityDto Priority { get; set; }    

        public DateTime CreationDate { get; set; }

        public DateTime DesiredPickupDate { get; set; }

        public string Comments { get; set; }

        public  OrderStatus Status { get; set; }
    }
}
