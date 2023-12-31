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
                phone_number = c.PhoneNumber
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
    }
}