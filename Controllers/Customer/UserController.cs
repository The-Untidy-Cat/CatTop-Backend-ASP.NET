using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace asp.net.Controllers.CustomerController
{
    public class ChangePasswordForm
    {
        [Required]
        [JsonPropertyName("new_password")]
        public string Password { get; set; }
    }

    public class UpdateProfileForm
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [JsonPropertyName("phone_number")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        [RegularExpression(@"^(0|1)$", ErrorMessage = "Giới tính không hợp lệ")]
        public int Gender { get; set; }
    }

    [Route("v1/customer/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DbCtx _context;

        public UserController(DbCtx context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var user = HttpContext.Items["user"];
            var customer = await _context.Customers.Where(c => c.User.Username == user).Select(c => new
            {
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.PhoneNumber,
                c.DateOfBirth,
                c.Gender,
                c.User.Username
            }).FirstOrDefaultAsync();
            var cart = await _context.Carts.Where(c => c.CustomerID == customer.Id)
                .Select(c => new
                {
                    c.Id,
                    c.Amount,
                    variant = new
                    {
                        c.Variant.Id,
                        c.Variant.Name,
                        sale_price = c.Variant.SalePrice,
                        c.Variant.Discount,
                        standard_price = c.Variant.StandardPrice,
                        c.Variant.Image,
                        c.Variant.SKU,
                        c.Variant.State,
                        product = new
                        {
                            c.Variant.Product.Id,
                            c.Variant.Product.Name,
                            c.Variant.Product.Slug,
                            c.Variant.Product.Image,
                            c.Variant.Product.State
                        }
                    }
                })
                .ToListAsync();
            var response = new
            {
                code = 200,
                message = "Success",
                data = new
                {
                    user = new
                    {
                        id = customer.Id,
                        first_name = customer.FirstName,
                        last_name = customer.LastName,
                        email = customer.Email,
                        phone_number = customer.PhoneNumber,
                        customer.Gender,
                        date_of_birth = customer.DateOfBirth,
                        username = customer.Username
                    },
                    cart
                }
            };
            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<IEnumerable<Customer>>> ChangePassword([FromBody] ChangePasswordForm request)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "Yêu cầu không hợp lệ"
                };
                return BadRequest(response);
            }
            if (request.Password.Length < 6)
            {
                var response = new
                {
                    code = 400,
                    message = "Mật khẩu mới phải có ít nhất 6 kí tự"
                };
                return BadRequest(response);
            }
            var user = HttpContext.Items["user"];
            var customer = await _context.Customers.Where(c => c.User.Username == user)
                .Select(c => new
                {
                    c.User,
                    c.Id
                }).FirstOrDefaultAsync();
            customer.User.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "Đổi mật khẩu thành công"
            };
            return Ok(responseSuccess);
        }

        [HttpPut]
        public async Task<ActionResult<IEnumerable<Customer>>> UpdateProfile([FromBody] UpdateProfileForm request)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "Yêu cầu không hợp lệ",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                };
                return BadRequest(response);
            }
            var user = HttpContext.Items["user"];
            var customer = await _context.Customers.Where(c => c.User.Username == user)
                .FirstOrDefaultAsync();
            customer.UpdatedAt = DateTime.Now;
            if (request.FirstName != null)
            {
                customer.FirstName = request.FirstName;
            }
            if (request.LastName != null)
            {
                customer.LastName = request.LastName;
            }
            if (request.Email != null)
            {
                customer.Email = request.Email;
            }
            if (request.PhoneNumber != null)
            {
                customer.PhoneNumber = request.PhoneNumber;
            }
            if (request.DateOfBirth != null)
            {
                customer.DateOfBirth = request.DateOfBirth;
            }
            if (request.Gender != null)
            {
                customer.Gender = request.Gender;
            }
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "Cập nhật thông tin thành công",
                data = new
                {
                    user = new
                    {
                        first_name = customer.FirstName,
                        last_name = customer.LastName,
                        email = customer.Email,
                        gender = customer.Gender,
                        date_of_birth = customer.DateOfBirth,
                        phone_number = customer.PhoneNumber,
                        username = user
                    }
                }
            };
            return Ok(responseSuccess);
        }
    }
}
