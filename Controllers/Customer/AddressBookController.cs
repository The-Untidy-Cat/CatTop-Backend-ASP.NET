using asp.net.Data;
using asp.net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace asp.net.Controllers.CustomerController
{
    public class NewAddressBookForm
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("address_line")]
        public string AddressLine { get; set; }

        [Required]
        public int Province { get; set; }

        [Required]
        public int District { get; set; }

        [Required]
        public int Ward { get; set; }

        [Required]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
    }

    public class UpdateAddressBookForm
    {
        public string Name { get; set; }

        [JsonPropertyName("address_line")]
        public string AddressLine { get; set; }

        public int Province { get; set; }

        public int District { get; set; }

        public int Ward { get; set; }

        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
    }

    [Route("v1/customer/address")]
    [ApiController]
    public class AddressBookController : ControllerBase
    {
        private readonly DbCtx _context;

        public AddressBookController(DbCtx context)
        {
            _context = context;
        }

        // GET: api/AddressBook
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressBook>>> GetAddressBooks()
        {
            var user = HttpContext.Items["user"];
            var addressBooks = await _context.AddressBooks.Where(a => a.Customer.User.Username == user)
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    address_line = a.AddressLine,
                    a.Province,
                    a.District,
                    a.Ward,
                    a.Phone
                }).ToListAsync();
            var response = new
            {
                code = 200,
                message = "success",
                data = new
                {
                    address_books = addressBooks
                }
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<AddressBook>>> CreateAddress([FromBody] NewAddressBookForm request)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "fail",
                    data = new
                    {
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage))
                    }
                };
                return BadRequest(response);
            }
            var user = HttpContext.Items["user"];
            var customer = await _context.Customers.Where(c => c.User.Username == user).FirstOrDefaultAsync();
            var addressBook = new AddressBook
            {
                Name = request.Name,
                AddressLine = request.AddressLine,
                Province = request.Province,
                District = request.District,
                Ward = request.Ward,
                Phone = request.Phone,
                CustomerId = customer.Id
            };
            _context.AddressBooks.Add(addressBook);
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "success",
                data = new
                {
                    address_book = addressBook
                }
            };
            return Ok(responseSuccess);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<AddressBook>>> UpdateAddress(int id, [FromBody] UpdateAddressBookForm request)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "fail",
                    data = new
                    {
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage))
                    }
                };
                return BadRequest(response);
            }
            var user = HttpContext.Items["user"];
            var customer = await _context.Customers.Where(c => c.User.Username == user).FirstOrDefaultAsync();
            var addressBook = await _context.AddressBooks.Where(a => a.Id == id && a.CustomerId == customer.Id).FirstOrDefaultAsync();
            if (addressBook == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Địa chỉ không tồn tại" 
                };
                return NotFound(response);
            }
            if (request.Name != null)
            {
                addressBook.Name = request.Name;
            }
            if (request.AddressLine != null)
            {
                addressBook.AddressLine = request.AddressLine;
            }
            if (request.Province != 0)
            {
                addressBook.Province = request.Province;
            }
            if (request.District != 0)
            {
                addressBook.District = request.District;
            }
            if (request.Ward != 0)
            {
                addressBook.Ward = request.Ward;
            }
            if (request.Phone != null)
            {
                addressBook.Phone = request.Phone;
            }
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "success",
                data = new
                {
                    address_book = new { 
                        addressBook.Id,
                        addressBook.Name,
                        address_line = addressBook.AddressLine,
                        addressBook.Province,
                        addressBook.District,
                        addressBook.Ward,
                        addressBook.Phone
                    }
                }
            };
            return Ok(responseSuccess);
        }
    }
}
