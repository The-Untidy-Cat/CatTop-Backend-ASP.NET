using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Services;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using asp.net.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace asp.net.Controllers.Auth
{
    public class LoginForm
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
        public async Task<ActionResult> CustomerLogin([FromBody] LoginForm request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var customer = await _context.Customers.Where(c => c.User.Username == request.Username).Select(c => new
            {
                c.FirstName,
                c.LastName,
                c.Email,
                c.PhoneNumber,
                c.DateOfBirth,
                c.Gender,
                c.User
            }).FirstAsync();
            if (customer == null || customer.User == null) return BadRequest(new
            {
                code = 400,
                message = "Không tìm thấy tài khoản"
            });
            var user = customer.User;
            if (user != null && !BCrypt.Net.BCrypt.Verify(request.Password, customer.User.Password))
                return BadRequest(new
                {
                    code = 400,
                    message = "Tài khoản/mật khẩu không đúng"
                });
            var token = AuthService.GenerateToken(user);
            return Ok(new
            {
                code = 200,
                data = new
                {
                    user = new
                    {
                        first_name = customer.FirstName,
                        last_name = customer.LastName,
                        phone_number = customer.PhoneNumber,
                        email = customer.Email,
                        gender = customer.Gender,
                        date_of_birth = customer.DateOfBirth,
                        username = user.Username
                    },
                    cart = new ArrayList(),
                    token
                }
            });
        }
        [HttpGet("csrf")]
        public async Task<ActionResult> GetCsrf()
        {
            var response = new
            {
                code = 200,
                request = new
                {
                    csrf = Guid.NewGuid().ToString(),
                },
            };
            return Ok(response);
        }
    }
}
