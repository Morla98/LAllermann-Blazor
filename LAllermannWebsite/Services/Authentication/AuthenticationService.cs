
using LAllermannShared.Models.DTOs;
using LAllermannWebsite.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text.Json;
namespace LAllermannWebsite.Services.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly Configuration _Configuration;
		private readonly HttpClient _httpClient;
		public AuthenticationService(IOptions<Configuration> Configuration)
		{
			_Configuration = Configuration.Value;
			_httpClient = new HttpClient()
			{
				BaseAddress = new Uri(_Configuration.Api_Address)
			};
		}
		
		
		private async Task<string?> GetTokenAsync(string username, string password)
		{
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/login", new { username, password });
			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync();
			}
			return null;
		}
		

		public async Task<bool> RegisterAsync(string username, string password, string confirmPassword, string ownerKey)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> ValidateTokenAsync(string token)
		{
			throw new NotImplementedException();
		}

		private class LAllermannToken
		{
			public UserToken User { get; set; }
			public string Token { get; set; } 
			public DateTime Nbf { get; set; }
			public DateTime Expiration { get; set; }
			public string Issuer { get; set; }
			public string Audience { get; set; }

			private JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
			public LAllermannToken(string token) { 
				Debug.WriteLine("Creating LAllermannToken");
				JwtSecurityToken jwtToken = _tokenHandler.ReadJwtToken(token);
				var tokenId = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
				Debug.WriteLine("TokenId: " + tokenId);
				var tokenName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
				Debug.WriteLine("TokenName: " + tokenName);
				var tokenRoleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "RoleId")?.Value;
				Debug.WriteLine("TokenRoleId: " + tokenRoleId);
				this.User = new UserToken()
				{
					Id = long.Parse(tokenId),
					Name = tokenName,
					RoleId = long.Parse(tokenRoleId)
				};
				this.Token = token;
				this.Nbf = jwtToken.ValidFrom;
				this.Expiration = jwtToken.ValidTo;
				this.Issuer = jwtToken.Issuer;
				this.Audience = jwtToken.Audiences.First();

				// check if token was valid by checking if no field is null
				if (this.User == null || this.User.Name == null || this.User.RoleId == null|| this.Token == null || this.Issuer == null || this.Audience == null)
				{
					throw new Exception("Invalid Token");
				}
				Debug.WriteLine("Finished Creating LAllermannToken");

			}
		}
				
		public async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(string username, string password)
		{
			string? token = await GetTokenAsync(username, password);
			if (token == null)
			{
				Debug.WriteLine("Incorrect Username or Password");
				throw new Exception("Incorrect Username or Password");
			}
			string? accessToken = JsonDocument.Parse(token).RootElement.GetProperty("accessToken").GetString();
			if (accessToken == null)
			{
				Debug.WriteLine("An error occurred while parsing the access token.");
				throw new Exception("An error occurred while parsing the access token.");
			}

			var LAToken = new LAllermannToken(accessToken);
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, LAToken.User.Name),
				new Claim(ClaimTypes.Role, LAToken.User.RoleId.ToString()),
				new Claim("nbf", LAToken.Nbf.ToString()),
				new Claim("exp", LAToken.Expiration.ToString()),
				new Claim("Token", token)
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(claimsIdentity);
			return principal;
		}

		public Task SignOut(string username, string password)
		{
			throw new NotImplementedException();
		}
	}
}
