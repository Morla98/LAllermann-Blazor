using LAllermannWebsite.Components;
using LAllermannWebsite.Models;
using LAllermannWebsite.Services.ApiServices.Authentication;
using LAllermannWebsite.Services.ApiServices.PasswordManager;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<Configuration>(builder.Configuration.GetSection("Configuration"));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<PasswordManagerService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
    {
		options.LoginPath = "/login";
		options.LogoutPath = "/logout";
		options.AccessDeniedPath = "/access-denied";
		options.SlidingExpiration = true;
		options.Cookie.Name = "auth_token";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(60);
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ReturnUrlParameter = "returnUrl";
	});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireClaim(ClaimTypes.Role, "1"));
    options.AddPolicy("User", policy =>
        policy.RequireClaim(ClaimTypes.Role, "2"));
});

builder.Services.AddCascadingAuthenticationState();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
