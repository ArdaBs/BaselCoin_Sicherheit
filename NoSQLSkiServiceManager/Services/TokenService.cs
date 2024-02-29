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
        private readonly TimeSpan _idleTimeout;
        private readonly TimeSpan _absoluteTimeout;

        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            _idleTimeout = TimeSpan.FromMinutes(double.Parse(config["Jwt:IdleTimeoutInMinutes"]));
            _absoluteTimeout = TimeSpan.FromDays(double.Parse(config["Jwt:AbsoluteTimeoutInDays"]));
        }

        public string CreateToken(string username, string role, string userId)
        {
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(userId))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userId));
            }
            else
            {
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
            }

            if (!string.IsNullOrEmpty(username))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, username));
            }
            else
            {
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");
            }

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            else
            {
                throw new ArgumentNullException(nameof(role), "Role cannot be null or empty.");
            }

            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_idleTimeout),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        public bool IsTokenExpired(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var iatClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat);
            if (iatClaim != null)
            {
                var iat = DateTimeOffset.FromUnixTimeSeconds(long.Parse(iatClaim.Value));
                if (iat.Add(_absoluteTimeout) <= DateTime.UtcNow)
                {
                    return true;
                }
                if (iat.Add(_idleTimeout) <= DateTime.UtcNow)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
