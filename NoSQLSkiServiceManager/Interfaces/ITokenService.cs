using NoSQLSkiServiceManager.Models;

namespace NoSQLSkiServiceManager.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(string username, string role, string userId);
    }
}
