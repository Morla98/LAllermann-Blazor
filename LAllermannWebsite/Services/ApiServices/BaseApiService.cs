using LAllermannWebsite.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace LAllermannWebsite.Services.ApiServices
{
	public class BaseApiService
	{
		protected readonly Configuration _Configuration;
		protected readonly HttpClient _httpClient;
        protected readonly AuthenticationStateProvider _authenticationStateProvider;
		protected readonly IHttpContextAccessor _httpContextAccessor;
        protected BaseApiService(IOptions<Configuration> Configuration, AuthenticationStateProvider authenticationStateProvider, IHttpContextAccessor httpContextAccessor)
		{
			_Configuration = Configuration.Value;
			_httpClient = new HttpClient()
			{
				BaseAddress = new Uri(_Configuration.Api_Address)
			};
			_authenticationStateProvider = authenticationStateProvider;
			_httpContextAccessor = httpContextAccessor;
		}
		protected void SetToken(string token)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		
	}
}
