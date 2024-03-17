using System.Security.Claims;

namespace LAllermannWebsite.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> ValidateTokenAsync(string token);
        Task<bool> RegisterAsync(string username, string password, string confirmPassword, string ownerKey);
        Task<ClaimsPrincipal> GetClaimsPrincipalAsync(string username, string password);
		Task SignOut(string username, string password);
	}
}
