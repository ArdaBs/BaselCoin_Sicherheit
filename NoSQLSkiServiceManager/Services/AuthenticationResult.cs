using NoSQLSkiServiceManager.Models;

namespace NoSQLSkiServiceManager.Services
{
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
        public int RemainingAttempts { get; set; }
        public AccountHolder Employee { get; set; }
    }
}
