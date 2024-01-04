using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Services;
using System.ComponentModel.DataAnnotations;
using asp.net.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Configuration;

namespace asp.net.Controllers.Auth
{
    public class LoginForm
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }

    public class CustomerRegistrationForm
    {
        [Required]
        [JsonPropertyName("first_name")]
        //[RegularExpression(@"[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ]+", ErrorMessage = "Tên không hợp lệ")]
        public string? FirstName { get; set; }

        [Required]
        [JsonPropertyName("last_name")]
        [RegularExpression(@"[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ]*$", ErrorMessage = "Tên không hợp lệ")]
        public string? LastName { get; set; }

        [Required]
        [JsonPropertyName("phone_number")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9]){6,}$")]
        public string? Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }
    }

    public class RequestOTPForm
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }

    public class ForgotPasswordForm
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^([0-9]){6,}$")]
        [JsonPropertyName("token")]
        public string OTP { get; set; }
    }

    public class ResetPasswordForm
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^([0-9]){6,}$")]
        [JsonPropertyName("token")]
        public string OTP { get; set; }

        [Required]
        [MinLength(6)]
        public string? Password { get; set; }
    }

    public class AuthService
    {
        public static string GenerateToken(User user, AuthSetting _authSetting)

        {
            var issuer = _authSetting.Jwt.Issuer;
            var audience = _authSetting.Jwt.Audience;
            var key = Encoding.ASCII.GetBytes(_authSetting.Jwt.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
             }),
                Expires = DateTime.UtcNow.AddDays(_authSetting.Jwt.ExpireDays),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }

        public static string ValidateToken(string token, AuthSetting _authSetting)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_authSetting?.Jwt?.Key);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = _authSetting.Jwt.Issuer,
                ValidAudience = _authSetting.Jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authSetting.Jwt.Key)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            }, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var user = jwtToken.Claims.FirstOrDefault(x => x.Type == "sub").Value;
            return user;
        }

        public static void AddTokenToCookie(HttpResponse response, string token, AuthSetting _authSetting)
        {
            response.Cookies.Append(_authSetting.Cookie.Name ?? "token", token, new CookieOptions
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
        private readonly IMailService _mailService;

        public AuthController(IOptions<AuthSetting> options, DbCtx context, IMailService _MailService)
        {
            _context = context;
            _authSetting = options.Value;
            _mailService = _MailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] CustomerRegistrationForm request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.Where(u => u.Username == request.Username).FirstOrDefaultAsync();
            if (user != null) return BadRequest(new
            {
                code = 400,
                message = "Tài khoản đã tồn tại"
            });
            var newUser = new User
            {
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                State = UserState.Active.ToString()
            };
            var newRole = new UserRole
            {
                User = newUser,
                RoleId = UserRoleEnum.Customer.ToString()
            };
            var newCustomer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                State = CustomerState.Active.ToString(),
                User = newUser
            };
            _context.Users.Add(newUser);
            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();
            user = newCustomer.User;
            var token = AuthService.GenerateToken(user, _authSetting);
            HttpResponse response = HttpContext.Response;
            AuthService.AddTokenToCookie(response, token, _authSetting);
            var emailData = new HTMLMailData
            {
                Email = request.Email,
                Subject = "Đăng ký tài khoản thành công",
                Content = "<p>Bạn vừa đăng ký tài khoản tại CatTop. Thông tin tài khoản của bạn:</p>" +
                "<p>- Tên đăng nhập: <strong>" + request.Username + "</strong></p>" +
                "<p>Sử dụng tài khoản này để đăng nhập vào CatTop.</p>"
            };
            await _mailService.SendHTMLMailAsync(emailData);
            return Ok(new
            {
                code = 200,
                data = new
                {
                    user = new
                    {
                        first_name = newCustomer.FirstName,
                        last_name = newCustomer.LastName,
                        phone_number = newCustomer.PhoneNumber,
                        email = newCustomer.Email,
                        gender = newCustomer.Gender,
                        date_of_birth = newCustomer.DateOfBirth,
                        username = user.Username
                    },
                    cart = new List<Array>()
                }
            });
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
            var employee = await _context.Employees.Where(e => e.User.Username == request.Username).Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Email,
                e.PhoneNumber,
                e.DateOfBirth,
                e.Gender,
                e.User
            }).FirstAsync();

            if (employee == null) return BadRequest(new
            {
                code = 400,
                message = "Không tìm thấy tài khoản"
            });
            var user = employee.User;
            if (user != null && !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
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
                        first_name = employee.FirstName,
                        last_name = employee.LastName,
                        phone_number = employee.PhoneNumber,
                        email = employee.Email,
                        gender = employee.Gender,
                        date_of_birth = employee.DateOfBirth,
                        username = user.Username
                    }
                }
            });
        }

        [HttpDelete("logout")]
        public async Task<ActionResult> Logout()
        {
            HttpResponse response = HttpContext.Response;
            response.Cookies.Delete(_authSetting.Cookie.Name);
            return Ok(new
            {
                code = 200,
                message = "Đăng xuất thành công"
            });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] RequestOTPForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = "Yêu cầu không hợp lệ"
                });
            }
            var customer = await _context.Customers.Where(c => c.Email == form.Email).FirstOrDefaultAsync();
            var employee = await _context.Employees.Where(e => e.Email == form.Email).FirstOrDefaultAsync();
            if (customer == null && employee == null) return BadRequest(new
            {
                code = 400,
                message = "Không tìm thấy tài khoản"
            });
            var user = customer?.User ?? employee?.User;
            var existingToken = await _context.PasswordResetTokens.Where(t => t.Email == form.Email).FirstOrDefaultAsync();
            if (existingToken != null)
            {
                _context.PasswordResetTokens.Remove(existingToken);
                await _context.SaveChangesAsync();
            }
            var OTP = new Random().Next(100000, 999999).ToString();
            var token = new PasswordResetToken
            {
                Email = form.Email,
                Token = OTP,
                CreatedAt = DateTime.Now
            };
            _context.PasswordResetTokens.Add(token);
            await _context.SaveChangesAsync();
            var emailData = new HTMLMailData
            {
                Email = form.Email,
                Subject = "Yêu cầu đặt lại mật khẩu",
                Content = "<p>Bạn vừa yêu cầu đặt lại mật khẩu tại CatTop.</p>" +
                "<p>Mã OTP của bạn là: <strong>" + OTP + "</strong>.</p>" +
                "<p>Mã OTP này sẽ hết hạn trong vòng <strong>" + _authSetting.ResetPassword.MaxAge + "</strong> giây. " +
                "Không chia sẻ mã OTP với bất kỳ ai, kể cả nhân viên của cửa hàng.</p>"
            };
            var isSent = await _mailService.SendHTMLMailAsync(emailData);
            if (!isSent)
            {
                _context.PasswordResetTokens.Remove(token);
                await _context.SaveChangesAsync();
                return BadRequest(new
                {
                    code = 400,
                    message = "Không thể gửi email"
                });
            }
            return Ok(new
            {
                code = 200,
                message = "Đã gửi email",
                data = new
                {
                    max_age = _authSetting.ResetPassword.MaxAge
                }
            });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = "Yêu cầu không hợp lệ"
                });
            }
            var token = await _context.PasswordResetTokens.Where(t => t.Email == form.Email).FirstOrDefaultAsync();
            if (token == null) return BadRequest(new
            {
                code = 400,
                message = "Mã OTP không hợp lệ"
            });
            DateTime createdAt = DateTime.Parse(token.CreatedAt.ToString());
            if (DateTime.Now >= createdAt.AddSeconds(_authSetting.ResetPassword.MaxAge))
            {
                _context.PasswordResetTokens.Remove(token);
                await _context.SaveChangesAsync();
                return BadRequest(new
                {
                    code = 400,
                    message = "Mã OTP đã hết hạn"
                });
            }
            var customer = await _context.Customers.Where(c => c.Email == form.Email).Select(c => c.User).FirstOrDefaultAsync();
            var employee = await _context.Employees.Where(e => e.Email == form.Email).Select(e => e.User).FirstOrDefaultAsync();
            var user = customer ?? employee;
            if (user == null) return BadRequest(new
            {
                code = 400,
                message = "Không tìm thấy tài khoản"
            });
            user.Password = BCrypt.Net.BCrypt.HashPassword(form.Password);
            _context.PasswordResetTokens.Remove(token);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                code = 200,
                message = "Đã đổi mật khẩu"
            });
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult> VerifyOTP([FromBody] ForgotPasswordForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = "Yêu cầu không hợp lệ"
                });
            }
            var token = await _context.PasswordResetTokens.Where(t => t.Email == form.Email).FirstOrDefaultAsync();
            if (token == null) return BadRequest(new
            {
                code = 400,
                message = "Mã OTP không hợp lệ"
            });
            DateTime createdAt = DateTime.Parse(token.CreatedAt.ToString());
            if (DateTime.Now >= createdAt.AddSeconds(_authSetting.ResetPassword.MaxAge))
            {
                _context.PasswordResetTokens.Remove(token);
                await _context.SaveChangesAsync();
                return BadRequest(new
                {
                    code = 400,
                    message = "Mã OTP đã hết hạn"
                });
            }

            return Ok(new
            {
                code = 200,
                message = "Mã OTP hợp lệ"
            });
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
