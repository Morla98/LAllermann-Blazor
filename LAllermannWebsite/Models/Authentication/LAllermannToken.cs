using LAllermannShared.Models.DTOs;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LAllermannWebsite.Models.Authentication
{
	public class LAllermannToken
	{
		public UserToken User { get; set; }
		public string Token { get; set; }
		public DateTime Nbf { get; set; }
		public DateTime Expiration { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; }

		private JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
		public LAllermannToken(string token)
		{
			Debug.WriteLine("Creating LAllermannToken");
			JwtSecurityToken jwtToken = _tokenHandler.ReadJwtToken(token);
			var tokenId = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
			Debug.WriteLine("TokenId: " + tokenId);
			var tokenName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
			Debug.WriteLine("TokenName: " + tokenName);
			var tokenRoleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "RoleId")?.Value;
			Debug.WriteLine("TokenRoleId: " + tokenRoleId);
			User = new UserToken()
			{
				Id = long.Parse(tokenId),
				Name = tokenName,
				RoleId = long.Parse(tokenRoleId)
			};
			Token = token;
			Debug.WriteLine("Token: " + Token);
			Nbf = jwtToken.ValidFrom;
			Expiration = jwtToken.ValidTo;
			Issuer = jwtToken.Issuer;
			Audience = jwtToken.Audiences.First();

			// check if token was valid by checking if no field is null
			if (User == null || User.Name == null || User.RoleId == null || Token == null || Issuer == null || Audience == null)
			{
				throw new Exception("Invalid Token");
			}
			Debug.WriteLine("Finished Creating LAllermannToken");
		}
	}
}
