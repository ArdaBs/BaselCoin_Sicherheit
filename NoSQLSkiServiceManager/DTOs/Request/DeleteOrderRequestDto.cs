using System.ComponentModel.DataAnnotations;

namespace NoSQLSkiServiceManager.DTOs.Requests
{
    public class DeleteOrderRequestDto
    {
        [Required]
        public string OrderId { get; set; }
    }
}
