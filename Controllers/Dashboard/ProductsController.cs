using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

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
        public class UpdateProductForm
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("slug")]
            public string Slug { get; set; }

            [JsonPropertyName("image")]
            public string Image { get; set; }

            [JsonPropertyName("brand_id")]
            public int BrandId { get; set; }

            [JsonPropertyName("state")]
            public string State { get; set; }

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
        public async Task<IActionResult> PutProduct(int id, [FromBody] UpdateProductForm request)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "Cập nhật sản phẩm thất bại",
                    data = new
                    {
                        errors = ModelState.Values.SelectMany(t => t.Errors.Select(e => e.ErrorMessage))
                    }
                };
                return BadRequest(response);
            }
            var item = await _context.Products
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
            if (item == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy sản phẩm"
                };
                return NotFound(response);
            }
            if (request.Name != null)
            {
                item.Name = request.Name;
            }
            if (request.Slug != null)
            {
                item.Slug = request.Slug;
            }
            if (request.BrandId != 0)
            {
                item.BrandId = request.BrandId;
            }
            if (request.Image != null)
            {
                item.Image = request.Image;
            }
            if (request.State != null)
            {
                item.State = request.State;
            }
            

            item.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "Cập nhật sản phẩm thành công",
                data = new
                {
                    item.Id,
                    item.Name,
                    item.Slug,
                    item.Description,
                    item.Image,
                    item.State,
                }
            };
            return Ok(responseSuccess);
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
