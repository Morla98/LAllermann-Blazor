using System.Security.Claims;

namespace LAllermannWebsite.Services.ApiServices.Authentication
{
	public interface IUserAuthenticationService
	{
		Task RegisterAsync(string username, string password, string confirmPassword, string ownerKey, string RoleId);
		Task<bool> ValidateTokenAsync();
		Task SignOut();
        Task SignIn(string username, string password);
        Task RefreshTokenIfNeededAsync();
	}
}
