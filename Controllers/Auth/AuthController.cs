using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using System.ComponentModel.DataAnnotations;
using NuGet.Protocol;

namespace asp.net.Controllers.Auth
{
    public class CustomerLoginForm
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
    [Route("v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DbCtx _context;
        public AuthController(DbCtx context)
        {
            _context = context;
        }
        [HttpPost("customer")]
        public async Task<ActionResult> CustomerLogin([FromBody] CustomerLoginForm data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.Where(u => u.Username == data.Username).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(data.Password, user?.Password))
            {
                return BadRequest(new
                {
                    code = 400,
                    message = "Username or password is incorrect"
                });
            }
            var customer = user.Customer.ToJson();
            var response = new
            {
                code = 200,
                data = user
            };
            return Ok(response);
        }
        [HttpGet("csrf")]
        public async Task<ActionResult> GetCsrf()
        {
            var response = new
            {
                code = 200,
                data = new
                {
                    csrf = Guid.NewGuid().ToString(),
                },
            };
            return Ok(response);
        }
    }
}
