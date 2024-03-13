using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LAllermannREST.Models;
using LAllermannREST.Services.PasswordHashers;

namespace LAllermannREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IPasswordHasher _passwordHasher;

       
        public UsersController(UserContext context)
        {
            _context = context;
        }

        
         
        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // TODO: Disable this endpoint for production
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }
        
        /*
        // GET for Debug purpose returning just Ok()
        [HttpGet("debug")]
        public IActionResult GetDebug()
        {
            return Ok();
        }

        // GET: api/Users 
        [HttpPost("create")]
        public async Task<IActionResult> PostUser(User user, string? secret)
        {
            if (secret == null) return BadRequest("Invalid");
            if (!secret.Equals(_TestSecret)) return BadRequest("Invalid2");
            user.Id = 0;
            user.Password = _passwordHasher.HashPassword(user.Password); // user.Password cant be null here because of the [Required] attribute
            user.CreatedAt = DateTime.Now;
            user.LastLogin = DateTime.Now;
            user.APIKEY = GenerateAPIKEY();
            var entityEntry = await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();

            user.Id = entityEntry.Entity.Id;
            
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }
        

        
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        private string GenerateAPIKEY()
        {

            return _passwordHasher.HashPassword(Guid.NewGuid().ToString() + Guid.NewGuid().ToString());
        }
       */
    }
}
