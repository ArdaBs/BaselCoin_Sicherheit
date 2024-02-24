using System.ComponentModel.DataAnnotations;

namespace NoSQLSkiServiceManager.DTOs.Request
{
    public class EmployeeCreateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }

}
