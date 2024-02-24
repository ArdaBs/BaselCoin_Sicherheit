using MongoDB.Bson.Serialization.Attributes;

namespace NoSQLSkiServiceManager.Models
{
    public class OrderStatus
    {
        [BsonElement("statusValue")]
        public string Status { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        public OrderStatus(string status, string description)
        {
            Status = status;
            Description = description;
        }

        public static OrderStatus Offen => new OrderStatus("Offen", "Die Bestellung ist offen und noch nicht bearbeitet.");
        public static OrderStatus InBearbeitung => new OrderStatus("In Bearbeitung", "Die Bestellung wird gerade bearbeitet.");
        public static OrderStatus Abgeschlossen => new OrderStatus("Abgeschlossen", "Die Bestellung wurde abgeschlossen.");
    }

}
