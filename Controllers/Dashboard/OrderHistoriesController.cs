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
    [Route("v1/dashboard/orderhistories")]
    public class OrderHistoriesController : ControllerBase
    {
        private readonly DbCtx _context;

        public OrderHistoriesController(DbCtx context)
        {
            _context = context;
        }

        // GET: api/OrderHistories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderHistories>>> GetOrderHistories()
        {
          if (_context.OrderHistories == null)
          {
              return NotFound();
          }
            return await _context.OrderHistories.ToListAsync();
        }

        // GET: api/OrderHistories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderHistories>> GetOrderHistories(int id)
        {
          if (_context.OrderHistories == null)
          {
              return NotFound();
          }
            var orderHistories = await _context.OrderHistories.FindAsync(id);

            if (orderHistories == null)
            {
                return NotFound();
            }

            return orderHistories;
        }

        // PUT: api/OrderHistories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderHistories(int id, OrderHistories orderHistories)
        {
            if (id != orderHistories.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(orderHistories).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderHistoriesExists(id))
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

        // POST: api/OrderHistories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderHistories>> PostOrderHistories(OrderHistories orderHistories)
        {
          if (_context.OrderHistories == null)
          {
              return Problem("Entity set 'DbCtx.OrderHistories'  is null.");
          }
            _context.OrderHistories.Add(orderHistories);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderHistoriesExists(orderHistories.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrderHistories", new { id = orderHistories.OrderId }, orderHistories);
        }

        // DELETE: api/OrderHistories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderHistories(int id)
        {
            if (_context.OrderHistories == null)
            {
                return NotFound();
            }
            var orderHistories = await _context.OrderHistories.FindAsync(id);
            if (orderHistories == null)
            {
                return NotFound();
            }

            _context.OrderHistories.Remove(orderHistories);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderHistoriesExists(int id)
        {
            return (_context.OrderHistories?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
