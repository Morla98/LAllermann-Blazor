using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LAllermannREST.Models;
using Microsoft.AspNetCore.Authorization;
using LAllermannREST.Services.TokenGenerators;
using System.Security.Claims;

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
            int userid = int.Parse(HttpContext.User.FindFirstValue("Id"));
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

            int tokenId = int.Parse(HttpContext.User.FindFirstValue("Id"));
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
            int TokenUserId = int.Parse(HttpContext.User.FindFirstValue("Id"));
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
            int tokenId = int.Parse(HttpContext.User.FindFirstValue("Id"));
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
