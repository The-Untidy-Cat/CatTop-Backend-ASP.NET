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
    [Route("v1/dashboard/orderitems")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly DbCtx _context;

        public OrderItemsController(DbCtx context)
        {
            _context = context;
        }

        // GET: api/OrderItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItems>>> GetOrderItems()
        {
            if (_context.OrderItems == null)
            {
                return NotFound();
            }
            var orderItems = await _context.OrderItems
                .Select(o => new
                {
                    id = o.Id,
                    order = o.Order,
                    variant = o.ProductVariant,
                    amount = o.Amount,
                    standard_price = o.StandardPrice,
                    total = o.Total,
                }).ToListAsync();
            return Ok(orderItems);
        }

        // GET: api/OrderItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItems>> GetOrderItems(int id)
        {
            if (_context.OrderItems == null)
            {
                return NotFound();
            }
            var orderItems = await _context.OrderItems.FindAsync(id);

            if (orderItems == null)
            {
                return NotFound();
            }

            return orderItems;
        }

        // PUT: api/OrderItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItems(int id, OrderItems orderItems)
        {
            if (id != orderItems.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderItems).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemsExists(id))
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

        // POST: api/OrderItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderItems>> PostOrderItems(OrderItems orderItems)
        {
            if (_context.OrderItems == null)
            {
                return Problem("Entity set 'DbCtx.OrderItems'  is null.");
            }
            _context.OrderItems.Add(orderItems);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderItems", new { id = orderItems.Id }, orderItems);
        }

        // DELETE: api/OrderItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItems(int id)
        {
            if (_context.OrderItems == null)
            {
                return NotFound();
            }
            var orderItems = await _context.OrderItems.FindAsync(id);
            if (orderItems == null)
            {
                return NotFound();
            }

            _context.OrderItems.Remove(orderItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderItemsExists(int id)
        {
            return (_context.OrderItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
