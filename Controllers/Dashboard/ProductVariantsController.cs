
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;

namespace asp.net.Controllers.Dashboard
{
    [Route("v1/dashboard/product_variants")]
    [ApiController]
    public class ProductVariantsController : ControllerBase
    {
        private readonly DbCtx _context;

        public ProductVariantsController(DbCtx context)
        {
            _context = context;
        }

        // GET: api/ProductVariants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVariants>>> GetProductVariants([FromQuery] SearchForm request)
        {
            if (_context.ProductVariants == null)
            {
                return NotFound();
            }
            var variants = _context.ProductVariants
                .Select(prod => new
                {
                    id = prod.Id,
                    name = prod.Name,
                    state = prod.State,
                    description = prod.Description,
                    sku = prod.SKU,
                    created_at = prod.Created_at,
                    prod_id = prod.ProductID,
                    standard_price = prod.StandardPrice,
                    tax_rate = prod.TaxRate,
                    discount = prod.Discount,
                    extra_fee = prod.ExtraFee,
                    cost_price = prod.CostPrice,
                    sale_price = prod.SalePrice
                });
            if (request.filter != null && request.keyword != null)
            {
                switch (request.filter)
                {
                    case "name":
                        variants = variants.Where(prod => prod.name.Contains(request.keyword));
                        break;
                    case "state":
                        variants = variants.Where(prod => prod.state.Contains(request.keyword));
                        break;
                    case "sku":
                        variants = variants.Where(prod => prod.sku.Contains(request.keyword));
                        break;
                    default:
                        break;
                }
            }
            var length = variants.Count();
            var records =
                await variants
                .Skip(request.offset)
                .Take(request.limit)
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
            //return await _context.Products.ToListAsync();
        }

        // GET: api/ProductVariants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariants>> GetProductVariants(int id)
        {
            if (_context.ProductVariants == null)
            {
                return NotFound();
            }
            var productVariants = await _context.ProductVariants.FindAsync(id);

            if (productVariants == null)
            {
                return NotFound();
            }

            return productVariants;
        }

        // PUT: api/ProductVariants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductVariants(int id, ProductVariants productVariants)
        {
            if (id != productVariants.Id)
            {
                return BadRequest();
            }

            _context.Entry(productVariants).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductVariantsExists(id))
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

        // POST: api/ProductVariants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductVariants>> PostProductVariants(ProductVariants productVariants)
        {
            if (_context.ProductVariants == null)
            {
                return Problem("Entity set 'DbCtx.ProductVariants'  is null.");
            }
            _context.ProductVariants.Add(productVariants);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductVariants", new { id = productVariants.Id }, productVariants);
        }

        // DELETE: api/ProductVariants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductVariants(int id)
        {
            if (_context.ProductVariants == null)
            {
                return NotFound();
            }
            var productVariants = await _context.ProductVariants.FindAsync(id);
            if (productVariants == null)
            {
                return NotFound();
            }

            _context.ProductVariants.Remove(productVariants);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductVariantsExists(int id)
        {
            return (_context.ProductVariants?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
