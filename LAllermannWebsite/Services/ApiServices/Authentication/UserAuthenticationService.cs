
using LAllermannShared.Models.DTOs;
using LAllermannWebsite.Models;
using LAllermannWebsite.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text.Json;
namespace LAllermannWebsite.Services.ApiServices.Authentication
{
	public class UserAuthenticationService : BaseApiService, IUserAuthenticationService
    {
       public UserAuthenticationService(IOptions<Configuration> Configuration, 
                                        AuthenticationStateProvider authenticationStateProvider,
										IHttpContextAccessor httpContextAccesor)
            : base(Configuration, authenticationStateProvider, httpContextAccesor) {}
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
		public async Task<bool> ValidateTokenAsync()
		{
            var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var token = authenticationState.User.FindFirst("Token")?.Value;
			this.SetToken(token);
			HttpResponseMessage response = await _httpClient.GetAsync("/api/validate-token");
			if (response.IsSuccessStatusCode) return true;
			return false;
		}

		private async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(string username, string password)
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
                new Claim("Token", LAToken.Token)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            return principal;
        }
        public async Task SignIn(string username, string password)
        {
            var principal = await GetClaimsPrincipalAsync(username, password);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
        public Task SignOut()
        {
            throw new NotImplementedException();
        }

        private bool NeedRefresh(string token)
        {
            var LAToken = new LAllermannToken(token);
            // Difference between the expiration date and the current date
            var difference = LAToken.Expiration - DateTime.Now;
            Debug.WriteLine("Difference: " + difference);
            throw new NotImplementedException();
        }

        public async Task<string> RefreshTokenIfNeeded()
        {
            var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var token = authenticationState.User.FindFirst("Token")?.Value;
            if(NeedRefresh(token))
            {
                Debug.WriteLine("Refreshing Token");
            }
            throw new NotImplementedException();
        }
    }
}
