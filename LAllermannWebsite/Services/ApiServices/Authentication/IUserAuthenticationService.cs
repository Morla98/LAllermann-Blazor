using System.Security.Claims;

namespace LAllermannWebsite.Services.ApiServices.Authentication
{
	public interface IUserAuthenticationService
	{
		Task<bool> RegisterAsync(string username, string password, string confirmPassword, string ownerKey);
		Task<bool> ValidateTokenAsync();
		Task SignOut();
        Task SignIn(string username, string password);
        Task<string> RefreshTokenIfNeeded();
	}
}
