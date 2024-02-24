namespace NoSQLSkiServiceManager.DTOs.Response
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
        public string Role { get; set; }
    }
}
