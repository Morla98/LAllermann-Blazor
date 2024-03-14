using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LAllermannREST.Services.TokenGenerators;
using System.Security.Claims;

using LAllermannREST.Data;
using LAllermannShared.Models.Entities;

namespace LAllermannREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordsController : ControllerBase
    {
        private readonly PasswordContext _context;
        private readonly AccessTokenGenerator _accessTokenGenerator;

        public PasswordsController(PasswordContext context, AccessTokenGenerator accessTokenGenerator)
        {
            _context = context;
            _accessTokenGenerator = accessTokenGenerator;
        }

        // GET: api/Passwords
        // TODO: Disable in production

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Password>>> GetPassword()
        {
            return await _context.Password.ToListAsync();
        }

        // GET: api/Passwords/User
        // Display all passwords of the user
        [HttpGet("User")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Password>>> GetPasswordByUser()
        {
            string? idClaim = HttpContext.User.FindFirstValue("Id");
            if (idClaim == null) return Unauthorized("Invalid token");
            int userid = int.Parse(idClaim);
            return await _context.Password.Where(p => p.UserId == userid).ToListAsync();
        }

        // PUT: api/Passwords/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutPassword(long id, Password password)
        {
            if (id != password.Id)
            {
                return BadRequest();
            }
            string? idClaim = HttpContext.User.FindFirstValue("Id");
            if (idClaim == null) return Unauthorized("Invalid token");
            int tokenId = int.Parse(idClaim);
            if (tokenId != password.UserId)
            {
                return Unauthorized("You are not allowed to modify passwords for other users");
            }

            _context.Entry(password).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PasswordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Passwords
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Password>> PostPassword(Password password)
        {
            string? idClaim = HttpContext.User.FindFirstValue("Id");
            if (idClaim == null) return Unauthorized("Invalid");
            int TokenUserId = int.Parse(idClaim);
            if (TokenUserId != password.UserId)
            {
                return Unauthorized("You are not allowed to create passwords for other users");
            }


            _context.Password.Add(password);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPassword", new { id = password.Id }, password);
        }

        // DELETE: api/Passwords/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePassword(long id)
        {
            var password = await _context.Password.FindAsync(id);
            if (password == null)
            {
                return BadRequest();
            }
            string? idClaim = HttpContext.User.FindFirstValue("Id");
            if (idClaim == null) return Unauthorized("Invalid");
            int tokenId = int.Parse(idClaim);
            if (tokenId != password.UserId)
            {
                return Unauthorized("You are not allowed to delete passwords for other users");
            }

            _context.Password.Remove(password);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        private bool PasswordExists(long id)
        {
            return _context.Password.Any(e => e.Id == id);
        }
    }
}
