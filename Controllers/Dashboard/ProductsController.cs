using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;

namespace asp.net.Controllers.Dashboard
{
    [Route("v1/dashboard/products")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly DbCtx _context;

        public ProductsController(DbCtx context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] SearchForm request)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var products = _context.Products
                .Select(prod => new
                {
                    id = prod.Id,
                    name = prod.Name,
                    state = prod.State,
                    description = prod.Description,
                    slug = prod.Slug,
                    created_at = prod.CreatedAt,
                    brand = new
                    {
                        id = prod.BrandId,
                        name = prod.Brand.Name
                    },
                    product_variant = new
                    {
                        id = prod.ProductVariants.Select(v => v.Id),
                        name = prod.ProductVariants.Select(v => v.Name),
                        sale_price = prod.ProductVariants.Select(v => v.SalePrice),
                        tax_rate = prod.ProductVariants.Select(v => v.TaxRate),
                        standard_price = prod.ProductVariants.Select(v => v.StandardPrice),
                    }
                });
            //.ToListAsync();
            if (request.filter != null && request.keyword != null)
            {
                switch (request.filter)
                {
                    case "name":
                        products = products.Where(prod => prod.name.Contains(request.keyword));
                        break;
                    case "state":
                        products = products.Where(prod => prod.state.Contains(request.keyword));
                        break;
                    default:
                        break;
                }
            }
            var length = products.Count();
            var records =
                await products
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
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'DbCtx.Products'  is null.");
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(long id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
