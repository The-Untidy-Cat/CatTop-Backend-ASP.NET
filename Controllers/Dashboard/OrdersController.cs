using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;

namespace asp.net.Controllers.Dashboard
{
    [Route("v1/dashboard/orders")]

    public class OrdersController : ControllerBase
    {
        private readonly DbCtx _context;

        public OrdersController(DbCtx context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders([FromQuery] SearchForm request)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }

            var orders = _context.Orders
                .Select(o => new
                {
                    id = o.Id,
                    //customer_id = o.CustomerId,
                    customer = new
                    {
                        customer_id = o.CustomerId,
                        first_name = o.Customer.FirstName,
                        last_name = o.Customer.LastName,
                    },
                    employee = new
                    {
                        employee_id = o.EmployeeId,
                        first_name = o.Employee.FirstName,
                        last_name = o.Employee.LastName,
                    },
                    items = new
                    {
                        id = o.OrderItems.Select(i => i.Id),
                        variant = o.OrderItems.Select(i => i.VariantId),
                        amount = o.OrderItems.Select(i => i.Amount),
                        sale_price = o.OrderItems.Select(i => i.SalePrice),
                        standard_price = o.OrderItems.Select(i => i.StandardPrice),
                        total = o.OrderItems.Select(i => i.Total),
                        order_id = o.OrderItems.Select(i => i.OrderId),
                    },
                    itemVariant = new
                    {
                        id = o.OrderItems.Select(i => i.ProductVariant.Id),
                        product_id = o.OrderItems.Select(i => i.ProductVariant.ProductID),
                        sku = o.OrderItems.Select(i => i.ProductVariant.SKU),
                    },
                    itemVariantProduct = new
                    {
                        id = o.OrderItems.Select(i => i.ProductVariant.Product.Id),
                        name = o.OrderItems.Select(i => i.ProductVariant.Product.Name),
                    }
                });
            

         
            if (request.filter != null && request.keyword != null)
            {
                switch (request.filter)
                {
                    case "customer_firstname":
                        orders = orders.Where(o => o.customer.first_name.ToString().Contains(request.keyword));
                        break;
                    case "employee_firstname":
                        orders = orders.Where(o => o.employee.first_name.ToString().Contains(request.keyword));
                        break;
                    case "product_name":
                        orders = orders.Where(o => o.itemVariantProduct.name.ToString().Contains(request.keyword));
                        break;
                    default:
                        break;
                }
            }
                var length = orders.Count();
            var records =
                await orders
                .Skip(request.offset).Take(request.limit)
                .ToListAsync();
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
            //return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'DbCtx.Orders'  is null.");
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
