using System.ComponentModel.DataAnnotations;

namespace NoSQLSkiServiceManager.DTOs.Request
{
    public class AccountHolderLoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
