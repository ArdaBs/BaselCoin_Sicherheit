using System.ComponentModel.DataAnnotations;

namespace NoSQLSkiServiceManager.DTOs.Request
{
    public class EmployeeUpdateDto
    {
        [Required]
        public string Username { get; set; }
    }

}
