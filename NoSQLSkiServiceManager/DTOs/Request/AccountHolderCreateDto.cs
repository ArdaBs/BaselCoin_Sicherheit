using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace NoSQLSkiServiceManager.DTOs.Request
{
    public class AccountHolderCreateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        // Kontoerstellungsinformationen
        [Range(0, double.MaxValue)]
        public double InitialBalance { get; set; } = 0.0;
    }
}
