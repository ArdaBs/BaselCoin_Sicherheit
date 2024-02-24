using NoSQLSkiServiceManager.Models;

namespace NoSQLSkiServiceManager.DTOs.Requests
{
    public class UpdateServiceOrderRequestDto
    {
        public string Comments { get; set; }
        public OrderStatus Status { get; set; }
    }
}
