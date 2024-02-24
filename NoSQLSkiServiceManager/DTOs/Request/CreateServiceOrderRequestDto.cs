using MongoDB.Bson;
using NoSQLSkiServiceManager.Models;
using System.ComponentModel.DataAnnotations;

namespace NoSQLSkiServiceManager.DTOs.Requests
{
    public class CreateServiceOrderRequestDto
    {
        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public string Comments { get; set; }

        [Required]
        public string ServiceTypeId { get; set; }

        [Required]
        public string PriorityId { get; set; }
    }
}

