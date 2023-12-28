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
                var query = _context.Products.Include(p => p.ProductVariants).Include(p => p.Brand)
                            .Where(q => q.State == ProductState.Published.ToString())
                            .Where(q => q.ProductVariants.Any(v => v.State == VariantState.Published.ToString()));
                var products = await query
                .Select(obj => new
                {
                    obj.Id,
                    name = obj.Name,
                    image = obj.Image,
                    created_at = obj.CreatedAt,
                    slug = obj.Slug,
                    discount = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().Discount,
                    sale_price = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().SalePrice,
                    standard_price = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().StandardPrice,
                    variants = obj.ProductVariants.Select(v => new
                    {
                        id = v.Id,
                        cpu = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Cpu.ToString(),
                        ram = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Ram.ToString(),
                        storage = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Storage.ToString(),
                        display = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Display.ToString(),
                        card = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Gpu.ToString()
                    }).ToList()
                })
                .ToListAsync();
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
                var query = _context.Products.Include(p => p.ProductVariants).Include(p => p.Brand)
                            .Where(p => p.Id == id)
                            .Where(q => q.State == ProductState.Published.ToString())
                            .Where(q => q.ProductVariants.Any(v => v.State == VariantState.Published.ToString()));
                var products = await query
                .Select(obj => new
                {
                    obj.Id,
                    name = obj.Name,
                    image = obj.Image,
                    created_at = obj.CreatedAt,
                    slug = obj.Slug,
                    discount = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().Discount,
                    sale_price = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().SalePrice,
                    standard_price = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().StandardPrice,
                    variants = obj.ProductVariants.Select(v => new
                    {
                        id = v.Id,
                        cpu = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Cpu.ToString(),
                        ram = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Ram.ToString(),
                        storage = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Storage.ToString(),
                        display = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Display.ToString(),
                        card = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Gpu.ToString()
                    }).ToList()
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
