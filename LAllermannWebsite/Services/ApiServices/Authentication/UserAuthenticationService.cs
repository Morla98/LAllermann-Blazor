
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


        public async Task RegisterAsync(string username, string password, string confirmPassword, string ownerKey, string RoleId)
        {
            // make a post request to the api/register with the username, password, confirmPassword, ownerKey and RoleId in the body
            var registerBody = new { username, password, confirmPassword, ownerKey, RoleId };
			var response = await _httpClient.PostAsJsonAsync("/api/register", registerBody);
            if (response.IsSuccessStatusCode) return;
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new Exception(responseBody);
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

        private ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
			var LAToken = new LAllermannToken(token);
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
            return GetClaimsPrincipalFromToken(accessToken);
        }

		public async Task SignIn(string username, string password)
        {
            var principal = await GetClaimsPrincipalAsync(username, password);
            var LAToken = new LAllermannToken(principal.FindFirst("Token")?.Value);

            Debug.WriteLine(LAToken.Nbf);
            Debug.WriteLine(LAToken.Expiration);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
				AllowRefresh = false,
                IssuedUtc = LAToken.Nbf,
                ExpiresUtc = LAToken.Expiration
		    };
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        }
        public async Task SignOut()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}

        private bool NeedRefresh(string token)
        {
            var LAToken = new LAllermannToken(token);
            var difference = LAToken.Expiration - DateTime.UtcNow;
            return difference.TotalMinutes < 5;
        }

        private async Task<string> RefreshTokenAsync(string token)
        {
            SetToken(token);
            HttpResponseMessage response = await _httpClient.GetAsync("/api/refresh-token");
            if (response.IsSuccessStatusCode)
            {
				return await response.Content.ReadAsStringAsync();
            }
            else
            {
                Debug.WriteLine(response.IsSuccessStatusCode);
				Debug.WriteLine(response.ToString());
				throw new Exception("An error occurred while refreshing the token.");
			}
            
        }
        public async Task RefreshTokenIfNeededAsync()
        {
            var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var token = authenticationState.User.FindFirst("Token")?.Value;
            if(!NeedRefresh(token)) return; // No refresh needed
			Debug.WriteLine("Token is nearing expiration. Initiating refresh process.");
			try
            {
				var newToken = await RefreshTokenAsync(token);
                Debug.WriteLine("New token: " + newToken);
                var principal = GetClaimsPrincipalFromToken(newToken);
                Debug.WriteLine("principal sucessfully created");
				await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
				return;
			}
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while refreshing the token: " + ex.Message);
            }
        }
    }
}
