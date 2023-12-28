using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.Build.Framework;
using System.Text.Json.Serialization;

namespace asp.net.Controllers.Dashboard
{
    

    [Route("v1/dashboard/ordersItem")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly DbCtx _context;

        public OrderItemsController(DbCtx context)
        {
            _context = context;
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
        //[HttpPost("{id}/items")]
        //public async Task<ActionResult<IEnumerable<OrderItems>>> CreateOrderItems([FromBody] NewOrderItemForm request)
        //{
        //    if(!ModelState.IsValid)
        //    {
        //        var response = new
        //        {
        //            code = 400,
        //            message = "Fail in CreateOrderItems",
        //            errors = ModelState.Values.SelectMany(t => t.Errors .Select(e => e.ErrorMessage))
        //        };
        //        return BadRequest(response);
        //    }
            
        //    var variant = await _context.ProductVariants.Where(ProductVariants => ProductVariants.Id == request.ProductVariantId).FirstOrDefaultAsync();
        //    var order = await _context.Orders.Where(Orders => Orders.Id == request.OrderId).FirstOrDefaultAsync();

        //    var orderItems = new OrderItems
        //    {
        //        OrderId = order.Id,
        //        VariantId = variant.Id,
        //        Amount = request.Quantity,
        //        StandardPrice = variant.StandardPrice,
        //        Total = variant.StandardPrice * request.Quantity,
        //        IsRefunded = false,
        //        Rating = null,
        //        SerialNumber = null,
        //        Review = null,
        //        CreatedAt = DateTime.Now,
        //    };
        //    await _context.OrderItems.AddAsync(orderItems);
        //    await _context.SaveChangesAsync();
        //    var responseSuccess = new
        //    {
        //        code = 200,
        //        message = "Success in CreateOrderItems",
        //        data = new
        //        {
        //            orderItems = new
        //            {
        //                order.Id,
        //                orderItems.VariantId,
        //                orderItems.Amount,

        //            }
        //        }
        //    };

        //    return Ok();
        //}


        private bool OrderItemsExists(int id)
        {
            return (_context.OrderItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
