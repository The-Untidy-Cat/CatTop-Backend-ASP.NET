using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace asp.net.Controllers.Dashboard
{
    public class SearchForm
    {
        [DefaultValue(10)]
        public int limit { get; set; }
        [DefaultValue(0)]
        public int offset { get; set; }
        public string? filter { get; set; }
        public string? keyword { get; set; }
        [JsonPropertyName("sort")]
        public string? sort { get; set; }
        [JsonPropertyName("order")]
        [DefaultValue("desc")]
        [RegularExpression("^(asc|desc)$")]
        public string? order { get; set; }

        [JsonPropertyName("start_date")]
        [DataType(DataType.Date)]
        public string? start_date { get; set; }

        [JsonPropertyName("end_date")]
        [DataType(DataType.Date)]
        public string? end_date { get; set; }

    }
    public class CustomerSearchForm : SearchForm
    {
    }
    [Route("v1/dashboard/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly DbCtx _context;

        public CustomersController(DbCtx context)
        {
            _context = context;
        }

        public class NewCustomerForm
        {
            [Required]
            [JsonPropertyName("Họ")]
            public string LastName { get; set; }
            [Required]
            [JsonPropertyName("Tên")]
            public string FirstName { get; set; }
            [Required]
            [JsonPropertyName("Email")]
            public string Email { get; set; }
            [Required]
            [JsonPropertyName("Số điện thoại")]
            public string PhoneNumber { get; set; }
            [Required]
            [JsonPropertyName("Ngày sinh")]
            public DateTime DoB { get; set; }
            [Required]
            [JsonPropertyName("Giới tính")]
            public int Gender { get; set; }
        }
        public class UpdateCustomerForm
        {
            [JsonPropertyName("Họ")]
            public string LastName { get; set; }
            [JsonPropertyName("Tên")]
            public string FirstName { get; set; }
            [JsonPropertyName("Email")]
            public string Email { get; set; }
            [JsonPropertyName("Số điện thoại")]
            public string PhoneNumber { get; set; }
            [JsonPropertyName("Ngày sinh")]
            public DateTime DoB { get; set; }
            [JsonPropertyName("Giới tính")]
            public int Gender { get; set; }
            [JsonPropertyName("Trạng thái")]
            public string State { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<Customer>> GetCustomers([FromQuery] CustomerSearchForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = "Bad Request",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            var customers = _context.Customers.Select(c => new
            {
                c.Id,
                first_name = c.FirstName,
                last_name = c.LastName,
                c.State,
                c.Email,
                phone_number = c.PhoneNumber,
                order_count = c.Orders
                            .Where(o => o.CustomerId == c.Id)
                            .Select(o => o.Id)
                            .Count()
            });

            if (form.filter != null && form.keyword != null)
            {
                switch (form.filter)
                {
                    case "first_name":
                        customers = customers.Where(c => c.first_name.Contains(form.keyword));
                        break;
                    case "last_name":
                        customers = customers.Where(c => c.last_name.Contains(form.keyword));
                        break;
                    case "state":
                        customers = customers.Where(c => c.State.Contains(form.keyword));
                        break;
                    case "email":
                        customers = customers.Where(c => c.Email.Contains(form.keyword));
                        break;
                    case "phone_number":
                        customers = customers.Where(c => c.phone_number.Contains(form.keyword));
                        break;
                    default:
                        break;
                }
            }
            var length = await customers.CountAsync();
            var records = await customers.Skip(form.offset).Take(form.limit).ToListAsync();
            var response = new
            {
                code = 200,
                message = "Success",
                data = new
                {
                    records,
                    length,
                    form.limit,
                    form.offset
                }
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = "Bad Request",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var customer = _context.Customers
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    id = c.Id,
                    first_name = c.FirstName,
                    last_name = c.LastName,
                    phone_number = c.PhoneNumber,
                    date_of_birth = c.DateOfBirth,
                    gender = c.Gender,
                    state = c.State,
                    user_id = c.UserId,
                    email = c.Email,
                    orders = c.Orders.Where(o => o.CustomerId == id).Select(o => new
                    {
                        id = o.Id,
                        shopping_method = o.ShoppingMethod,
                        payment_state = o.PaymentState,
                        state = o.State,
                        tracking_no = o.TrackingNo,
                        address_id = o.AddressId,
                        note = o.Note,
                        created_at = o.CreatedAt,
                        updated_at = o.UpdatedAt,
                        total = o.OrderItems.Where(i => i.OrderId == i.Id).Sum(i => i.Total)
                    })
                });

            if(customer == null)
            {
                return NotFound(new
                {
                    code = 404,
                    message = "Không tìm thấy thông tin"
                });
            }

            var records = await customer.ToListAsync();
            var response = new
            {
                code = 200,
                message = "get.Success",
                data = new
                {
                    records,
                }
            };
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerForm request)
        {
            var item = await _context.Customers
                .Where(c => c.Id == id).FirstOrDefaultAsync();
            if (item == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không có thông tin"
                };
                return NotFound(response);
            }
            if (request.FirstName != null)
            {
                item.FirstName = request.FirstName;
            }
            if (request.LastName != null)
            {
                item.LastName = request.LastName;
            }
            if (request.Email != null)
            {
                item.Email = request.Email;
            }
            if (request.PhoneNumber != null)
            {
                item.PhoneNumber = request.PhoneNumber;
            }
            if (request.DoB != null)
            {
                item.DateOfBirth = request.DoB;
            }
            if (request.Gender != null)
            {
                item.Gender = request.Gender;
            }
            if (request.State != null)
            {
                item.State = request.State;
            }

            item.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            var customer = await _context.Customers
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    id = c.Id,
                    first_name = c.FirstName,
                    last_name = c.LastName,
                    email = c.Email,
                    phone_number = c.PhoneNumber,
                    date_of_birth = c.DateOfBirth,
                    gender = c.Gender,
                    state = c.State,
                    user_id = c.UserId,
                    created_at = c.CreatedAt,
                    updated_at = c.UpdatedAt,
                    order_count = c.Orders.Where(o => o.CustomerId == id).Select(o => o.Id).Count(),
                }).ToListAsync();

            var responsesuccess = new
            {
                code = 200,
                message = "messages.update.success",
                data = new
                {
                    customer,
                }
            };

            return Ok(responsesuccess);
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Customer>>> CreateCustomer([FromBody] NewCustomerForm request)
        {
            var user = new User
            {
                Username = request.PhoneNumber,
                Password = "12345678",
                State = "active",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var userID = await _context.Users
                        .Where(u => u.Username == user.Username)
                        .Select(u => u.Id).ToListAsync();

            var userRole = new UserRole
            {
                UserId = userID.First(),
                RoleId = "Customer",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();

            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DoB,
                Gender = request.Gender,
                State = CustomerState.Active.ToString(),
                UserId = userID.First(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            var resp = new
            {
                code = 200,
                message = "success",
                data = new
                {
                    state = customer.State,
                    user_id = customer.UserId,
                    first_name = customer.FirstName,
                    last_name = customer.LastName,
                    email = customer.Email,
                    phone_number = customer.PhoneNumber,
                    date_of_birth = customer.DateOfBirth,
                    gender = customer.Gender,
                    updated_at = customer.UpdatedAt,
                    create_at = customer.CreatedAt,
                    id = customer.Id,
                    order_count = 0,
                }
            };
            return Ok(resp);
        }
        
        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(c => c.Id == id)).GetValueOrDefault();
        }
    }
}