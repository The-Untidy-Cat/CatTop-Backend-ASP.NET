using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;

namespace asp.net.Controllers.Dashboard
{
    [Route("v1/dashboard")]
    [ApiController]

    public class SearchForm
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public string filter { get; set; }
        public string keyword { get; set; }
    }

    public class CustomersController : ControllerBase
    {
        private readonly DbCtx _context;

        public CustomersController(DbCtx context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet("/")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers([FromQuery] SearchForm request)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customers = _context.Customers.Select(c => new
            {
                id = c.Id,
                first_name = c.FirstName,
                last_name = c.LastName,
                email = c.Email,
                phone_number = c.PhoneNumber,
                c.User
            });
            if (request.filter != null && request.keyword != null)
            {
                switch (request.filter)
                {
                    case "name":
                        customers = customers.Where(c => c.first_name.Contains(request.keyword));
                        break;
                    case "phone_number":
                        customers = customers.Where(c => c.phone_number.Contains(request.keyword));
                        break;
                    default:
                        break;
                }
            }
            var length = customers.Count();
            var records =
                await customers.Skip(request.offset).Take(request.limit).ToListAsync();
            var response = new
            {
                code = 200,
                data = new
                {
                    records,
                    request.offset,
                    request.limit,
                    length,
                }
            };
            return Ok(response);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'DbCtx.Customers'  is null.");
            }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
