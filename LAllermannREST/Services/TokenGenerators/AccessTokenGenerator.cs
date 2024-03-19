using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using LAllermannShared.Models.Entities;
using LAllermannREST.Models;
using Newtonsoft.Json.Linq;

namespace LAllermannREST.Services.TokenGenerators
{
    public class AccessTokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;
        public AccessTokenGenerator(IOptions<AuthenticationConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public String GenerateToken(User user)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.JwtSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("APIKEY", user.APIKEY), //
                new Claim("RoleId", user.RoleId.ToString())
            };
            JwtSecurityToken token = new JwtSecurityToken(
                _configuration.JwtIssuer,
                _configuration.JwtAudience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(_configuration.JwtExpireTime),
                credentials

            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // Decode Token without validation
        public JwtSecurityToken DecodeToken(string token)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token);
        }

        public User getUserFromToken(string token)
        {
            var JWTToken = DecodeToken(token);
			var userId = JWTToken.Claims.First(c => c.Type == "Id").Value;
			var userName = JWTToken.Claims.First(c => c.Type == ClaimTypes.Name).Value;
			var RoleId = JWTToken.Claims.First(c => c.Type == "RoleId").Value;
			var APIKEY = JWTToken.Claims.First(c => c.Type == "APIKEY").Value;

            return new User
            {
				Id = int.Parse(userId),
				Name = userName,
				RoleId = int.Parse(RoleId),
				APIKEY = APIKEY
			};
		}
    }
}
