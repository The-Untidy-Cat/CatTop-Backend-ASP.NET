using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using System.ComponentModel.DataAnnotations;
using NuGet.Protocol;

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
            //var user = await _context.Users
            //    .Where(u => u.Username == request.Username)
            //    .FirstAsync();
            //if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user?.Password))
            //{
            //    return BadRequest(new
            //    {
            //        code = 400,
            //        message = "Username or password is incorrect"
            //    });
            //}
            //var customer = await _context.Customers.Select(c => new
            //{
            //    user_id = c.UserId,
            //    first_name = c.FirstName,
            //    last_name = c.LastName,
            //    email = c.Email,
            //    phone_number = c.PhoneNumber,
            //    gender = c.Gender,
            //    date_of_birth = c.DateOfBirth
            //}).Where(c => c.user_id == user.Id).FirstAsync();

            //if (customer == null)
            //{
            //    return BadRequest(new
            //    {
            //        code = 400,
            //        message = "Customer not found"
            //    });
            //}
            //var response = new
            //{
            //    code = 200,
            //    request = new
            //    {
            //        user = new
            //        {
            //            customer.email,
            //            customer.first_name,
            //            customer.last_name,
            //            customer.phone_number,
            //            customer.gender,
            //            customer.date_of_birth,
            //            username = user.Username
            //        }
            //    }
            //};
            var user = await _context.Users.Where(u => u.Username == request.Username).FirstAsync();
            var customer = user?.Customer;
            var response = new
            {
                user,
                customer
            };
            return Ok(response);
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
