using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            {
                var query = _context.Products.Include(p => p.ProductVariants)
                            .Where(q => q.State == ProductState.Published.ToString())
                            .Where(q => q.ProductVariants.Any(v => v.State == VariantState.Published.ToString()));
                var products = await query
                .Select(obj => new
                {
                    obj.Id,
                    name = obj.Name,
                    state = obj.State,
                    variant_count = obj.ProductVariants.Count(),
                    discount = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().Discount,
                    sale_price = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().SalePrice,
                    standard_price = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().StandardPrice,
                    brands = new
                    {
                        id = obj.Brand.Id,
                        name = obj.Brand.Name,
                        product_count = obj.Brand.Products.Count()
                    }
                }).ToListAsync();

                var response = new
                {
                    code = 200,
                    data = new
                    {
                        products
                    }
                };
                return Ok(response);
            }
        }
        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            {
                var query = _context.Products.Include(p => p.ProductVariants)
                            .Where(p => p.Id == id)
                            .Where(q => q.State == ProductState.Published.ToString())
                            .Where(q => q.ProductVariants.Any(v => v.State == VariantState.Published.ToString()));
                var products = await query
                .Select(obj => new
                {
                    id = obj.Id,
                    name = obj.Name,
                    slug = obj.Slug,
                    description = obj.Description,
                    image = obj.Image,
                    state = obj.State,
                    variants = new {
                        id = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().Id,
                        name = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().Name,
                        sku = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().SKU,
                        standard_price = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().StandardPrice,
                        sale_price = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().SalePrice,
                        discount = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().Discount,
                        state = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().State,
                        sold = obj.ProductVariants.Select(v => v.OrderItems.Sum(v => v.Amount)).FirstOrDefault()
                    },
                     brands = new
                    {
                        id = obj.Brand.Id,
                        name = obj.Brand.Name,
                        image = obj.Brand.Image,
                        product_count = obj.Brand.Products.Count()
                    }
                })
                .ToListAsync();
                if (products == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        message = "Không tìm thấy đơn hàng"
                    });
                }
                var response = new
                {
                    code = 200,
                    data = new
                    {
                        products
                    }
                };

                return Ok(response);
            }
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
