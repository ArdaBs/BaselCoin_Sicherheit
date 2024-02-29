using NoSQLSkiServiceManager.Interfaces;

namespace NoSQLSkiServiceManager.DTOs.Response
{
    public class AccountHolderResponseDto : IResponseDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public bool IsLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
        public string Role { get; set; }
        public double Balance { get; set; }
    }
}

