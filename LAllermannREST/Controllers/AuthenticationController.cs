using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using LAllermannREST.Data;
using LAllermannREST.Models.Responses;
using LAllermannREST.Services.PasswordHashers;
using LAllermannREST.Services.TokenGenerators;
using LAllermannShared.Models.Entities;
using LAllermannREST.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace LAllermannREST.Controllers
{

    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationConfiguration _configuration;
        private readonly UserContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AccessTokenGenerator _accessTokenGenerator;
        private Random _random = new Random();


        public AuthenticationController(IOptions<AuthenticationConfiguration> configuration, UserContext context, IPasswordHasher passwordHasher, AccessTokenGenerator accessTokenGenerator)
        {
            _configuration = configuration.Value;
            _context = context;
            _passwordHasher = passwordHasher;
            _accessTokenGenerator = accessTokenGenerator;
        }

        private string GenerateAPIKEY()
        {

            const int length = 64;
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            char[] apiKey = new char[length];
            for (int i = 0; i < length; i++)
            {
                apiKey[i] = validChars[_random.Next(validChars.Length)];
            }

            return new string(apiKey);
        }
        [HttpGet("api/validate-token")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            return Ok("Token is valid");
        }

        [HttpPost("api/register")]
        public async Task<IActionResult> Register([FromBody] Models.Requests.RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return BadRequest("Passwords do not match");
            }
            if (registerRequest.OwnerKey != _configuration.OwnerKey)
            {
                return BadRequest("Invalid owner key");
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == registerRequest.RoleId);
            if (role == null)
            {
				return BadRequest("Invalid role");
			}

            var user = new User
            {
                Name = registerRequest.Username,
                Password = _passwordHasher.HashPassword(registerRequest.Password),
                CreatedAt = DateTime.Now,
                LastLogin = DateTime.Now,
                APIKEY = GenerateAPIKEY(),
                RoleId = registerRequest.RoleId
            };

            //Check if user already exists
            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Name.Equals(user.Name));
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }



            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok("User registered");
        }

        [HttpPost("api/login")]
        public async Task<IActionResult> Login([FromBody] Models.Requests.LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.User.FirstOrDefaultAsync(u => u.Name.Equals(loginRequest.Username));
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }
            if (!_passwordHasher.VerifyPassword(loginRequest.Password, user.Password))
            {
                return Unauthorized("Invalid username or password");
            }
            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            string accessToken = _accessTokenGenerator.GenerateToken(user);

            AuthenticationResponse responsebody = new AuthenticationResponse
            {
                AccessToken = accessToken,
                
            };
            return Ok(responsebody);
        }

        [HttpGet("/api/refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var authHeader = Request.Headers["Authorization"];
            var token = authHeader.ToString().Split(" ")[1];
            
            var user = _accessTokenGenerator.getUserFromToken(token);
            if (user == null)
            {
                return Unauthorized("Invalid");
            }
            string newToken = _accessTokenGenerator.GenerateToken(user);
			return Ok(newToken);
		}
	}
}
