
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using System.Text.Json.Serialization;

namespace asp.net.Controllers.Dashboard
{
    [Route("v1/dashboard/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly DbCtx _context;

        public BrandsController(DbCtx context)
        {
            _context = context;
        }

        public class UpdateBrandForm
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("image")]
            public string Image { get; set; }

            [JsonPropertyName("state")]
            public string State { get; set; }

        }
        // GET: api/Brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands([FromQuery] SearchForm request)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }

            var query = _context.Brands.Include(p => p.Products)
                        .Where(q => q.State == BrandState.Active.ToString());
            var brands = await query
            .Select(obj => new
            {
                id = obj.Id,
                name = obj.Name,
                //image = obj.Image,
                state = obj.State,
                product_count = obj.Products
                            .Where(p => p.BrandId == obj.Id)
                            .Select(p => p.ProductVariants.Select(v => v.ProductID))
                            .Count()
            }).ToListAsync();

            var length = brands.Count();
            var response = new
            {
                code = 200,
                data = new
                {
                    brands,
                    request.offset,
                    request.limit,
                    length,
                }
            };
            return Ok(response);

        }

        // GET: api/Brands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            if (_context.Brands == null)
            {
                return NotFound(new
                {
                    code = 404,
                    message = "Lỗi Context"
                });
            }

            var query = _context.Brands
                        .Include(b => b.Products)
                        .Where(b => b.Id == id)
                        .Where(b => b.State == BrandState.Active.ToString());
            var brands = await query
            .Where(b => b.Id == id)
            .Select(obj => new
            {
                id,
                name = obj.Name,
                slug = obj.Slug == null ? "null" : obj.Slug,
                description = obj.Description,
                image = obj.Image,
                state = obj.State,
                parent_id = obj.ParentId,
                created_at = obj.CreatedAt,
                updated_at = obj.UpdatedAt,
                product_count = obj.Products
                            .Where(p => p.BrandId == id)
                            .Select(p => p.ProductVariants.Select(v => v.ProductID))
                            .Count()
            }).ToListAsync();

            if (brands == null)
            {
                return NotFound(new
                {
                    code = 404,
                    message = "Không tìm thấy thương hiệu"
                });
            }
            var response = new
            {
                code = 200,
                data = new
                {
                    brands
                }
            };

            return Ok(response);
        }

        // PUT: api/Brands/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBrand(int id, [FromBody] UpdateBrandForm request)
        {
            var item = await _context.Brands
                .Where(b => b.Id == id).FirstOrDefaultAsync();
            if (item == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy thương hiệu"
                };
                return NotFound(response);
            }
            if (request.Name != null)
            {
                item.Name = request.Name;
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
                message = "Cập nhật thương hiệu thành công",
                data = new
                {
                    
                    id = item.Id,
                    name = item.Name,
                    slug = item.Slug,
                    description = item.Description,
                    image = item.Image,
                    state = item.State,
                    parent_id = item.ParentId,
                    created_at = item.CreatedAt,
                    updated_at = item.UpdatedAt,
                }
            };
            return Ok(responseSuccess);
        }
        private bool BrandExists(int id)
        {
            return (_context.Brands?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
