using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using LAllermannREST.Services.PasswordHashers;
using LAllermannREST.Services.TokenGenerators;
using LAllermannREST.Data;
using LAllermannREST.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

// Load Configuration
builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("Configuration"));

builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration.GetSection("Configuration")["JWTSecret"];
        if (jwtSecret == null)
        {
            throw new Exception("JWTSecret not found in configuration");
        }
        SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetSection("Configuration")["JwtIssuer"],
            ValidAudience = builder.Configuration.GetSection("Configuration")["JwtAudience"],
            IssuerSigningKey = signingKey
        };
        options.Audience = "LAllermannREST";
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserContext>();
builder.Services.AddDbContext<PasswordContext>();

builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton<AccessTokenGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
