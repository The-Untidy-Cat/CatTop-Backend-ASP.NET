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
using Microsoft.Extensions.Options;

namespace asp.net.Controllers.Auth
{
    public class LoginForm
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }

    public class AuthService
    {
        public static string GenerateToken(User user, AuthSetting _authSetting)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSetting?.Jwt?.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _authSetting.Jwt.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                    };

            var token = new JwtSecurityToken(_authSetting.Jwt.Issuer,
                _authSetting.Jwt.Audience,
                claims,
                expires: DateTime.Now.AddDays(_authSetting.Jwt.ExpireDays),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static string ValidateToken(string token, AuthSetting _authSetting)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authSetting.Jwt.Key);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _authSetting.Jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = _authSetting.Jwt.Audience,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.First(x => x.Type == "unique_name").Value;
        }
        public static void AddTokenToCookie(HttpResponse response, string token, AuthSetting _authSetting)
        {
            response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = _authSetting.Cookie.HttpOnly,
                SameSite = _authSetting.Cookie.SameSite,
                Expires = DateTime.Now.AddDays(_authSetting.Cookie.MaxAge),
                Secure = _authSetting.Cookie.Secure
            });
        }
    }

    [Route("v1/auth")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly DbCtx _context;
        private readonly AuthSetting _authSetting;

        public AuthController(IOptions<AuthSetting> options, DbCtx context)
        {
            _context = context;
            _authSetting = options.Value;
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
            var token = AuthService.GenerateToken(user, _authSetting);
            HttpResponse response = HttpContext.Response;
            AuthService.AddTokenToCookie(response, token, _authSetting);
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
                    cart = new List<Array>()
                }
            });
        }

        [HttpPost("dash")]
        public async Task<ActionResult> DashLogin([FromBody] LoginForm request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Employees.Where(e => e.User.Username == request.Username).Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.User.Username,
                e.User.Password,
                e.User.UserRoles
            }).FirstAsync();

            if (user == null) return BadRequest(new
            {
                code = 400,
                message = "Không tìm thấy tài khoản"
            });

            if (user != null && !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return BadRequest(new
                {
                    code = 400,
                    message = "Tài khoản/mật khẩu không đúng"
                });
            //var token = AuthService.GenerateToken(user, _authSetting);
            //HttpResponse response = HttpContext.Response;
            //AuthService.AddTokenToCookie(response, token, _authSetting);
            return Ok(new
            {
                code = 200,
                data = new
                {
                    //user = new
                    //{
                    //    username = user.Username
                    //}
                    user
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
