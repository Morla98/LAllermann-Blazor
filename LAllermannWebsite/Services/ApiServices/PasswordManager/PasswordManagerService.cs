using LAllermannShared.Models.Entities;
using LAllermannWebsite.Models;
using LAllermannWebsite.Models.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LAllermannWebsite.Services.ApiServices.PasswordManager
{
	public class PasswordManagerService : BaseApiService
	{
		private  List<Password> passwords = new List<Password>();
		public PasswordManagerService(IOptions<Configuration> Configuration, AuthenticationStateProvider authenticationStateProvider, IHttpContextAccessor httpContextAccessor) : base(Configuration, authenticationStateProvider, httpContextAccessor) 
		{
			
		}

		public async Task<List<Password>> GetPasswords()
		{
			if (passwords.Count == 0)
			{
				await InitializePasswords();
			}
			return passwords;
		}

		// Debug Print all passwords in passwords
		public void PrintPasswords()
		{
			foreach (Password password in passwords)
			{
				System.Diagnostics.Debug.WriteLine(password.Id);
				System.Diagnostics.Debug.WriteLine(password.Username);
				System.Diagnostics.Debug.WriteLine(password.Title);
				System.Diagnostics.Debug.WriteLine(password.Notes);
			}
		}

		private async Task CheckIdentity()
		{
			var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			var token = authenticationState.User.FindFirst("Token")?.Value;
			if (token == null) return; // No token found
			SetToken(token);
		}

		private async Task<long?> getUserId()
		{
			var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var LAToken = new LAllermannToken(authenticationState.User.FindFirst("Token")?.Value);
			var user = LAToken.User;
			if (user == null) return -1;
			var userId = user.Id;
			return userId;
		}
		public async Task AddPassword(Password password)
		{
			password.UserId = (long)await getUserId();
			await CheckIdentity();
			var content = new StringContent(JsonSerializer.Serialize(password), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _httpClient.PostAsync("/api/passwords", content);
			if (response.IsSuccessStatusCode)
			{
                passwords.Add(password);
			}else
			{
                var responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString);
                System.Diagnostics.Debug.WriteLine("Add Failed");
			}
		}

		public async Task EditPassword(Password password)
		{
			await CheckIdentity();
			var content = new StringContent(JsonSerializer.Serialize(password), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _httpClient.PutAsync("/api/passwords/" + password.Id, content);
			if (response.IsSuccessStatusCode)
			{
				Password? oldPassword = passwords.Find(p => p.Id == password.Id);
				if (oldPassword != null)
				{
					oldPassword.Title = password.Title;
					oldPassword.Username = password.Username;
					oldPassword.UserPassword = password.UserPassword;
					oldPassword.URL = password.URL;
					oldPassword.Notes = password.Notes;
					oldPassword.UserId = password.UserId;
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Edit Failed");
				System.Diagnostics.Debug.WriteLine(response.StatusCode);
				System.Diagnostics.Debug.WriteLine(response.ReasonPhrase);
			}
		}

		public async Task DeletePassword(long id)
		{
			await CheckIdentity();
			HttpResponseMessage response = await _httpClient.DeleteAsync("/api/passwords/" + id);
			if (response.IsSuccessStatusCode)
			{
				Password? password = passwords.Find(p => p.Id == id);
				if (password != null)
				{
					passwords.Remove(password);
				}
			}
		}

		private async Task InitializePasswords()
		{
			await CheckIdentity();
			HttpResponseMessage response = await _httpClient.GetAsync("/api/passwords/User");
			if (response.IsSuccessStatusCode)
			{
				var responseString = await response.Content.ReadAsStringAsync();	
				var jsonObject = JsonDocument.Parse(responseString);
				if (jsonObject == null)
				{
					System.Diagnostics.Debug.WriteLine("jsonObject is null");
					return;
				}
				
				foreach (var item in jsonObject.RootElement.EnumerateArray())
				{
					
					Password password = new Password()
					{
						Id = item.GetProperty("id").GetInt32(),
						Title = item.GetProperty("title").GetString(),
						Username = item.GetProperty("username").GetString(),
						UserPassword = item.GetProperty("userPassword").GetString(),
						URL = item.GetProperty("url").GetString(),
						Notes = item.GetProperty("notes").GetString(),
						UserId = item.GetProperty("userId").GetInt32()
					};
					passwords.Add(password);
					
				}
			}
		}

		
	}
}
