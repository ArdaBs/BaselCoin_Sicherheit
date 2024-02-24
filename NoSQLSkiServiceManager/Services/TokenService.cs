using Microsoft.IdentityModel.Tokens;
using NoSQLSkiServiceManager.Interfaces;
using NoSQLSkiServiceManager.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoSQLSkiServiceManager.Services
{
    /// <summary>
    /// Service responsible for generating JWT tokens for authentication purposes.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        /// <summary>
        /// Initializes a new instance of the TokenService with the symmetric security key.
        /// </summary>
        /// <param name="config">Configuration containing JWT settings.</param>
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        }

        /// <summary>
        /// Creates a JWT token for a given username.
        /// </summary>
        /// <param name="username">The username for which the token is being created.</param>
        /// <returns>A JWT token as a string.</returns>
        public string CreateToken(string username, string role, string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, username),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
