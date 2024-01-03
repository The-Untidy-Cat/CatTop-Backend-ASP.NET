
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace asp.net.Controllers.Dashboard
{
    public class NewBrandForm
    {
        [Required]
        public string? name { get; set; }

        [Required]
        [Url]
        public string image { get; set; }
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
    [Route("v1/dashboard/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly DbCtx _context;

        public BrandsController(DbCtx context)
        {
            _context = context;
        }
        // GET: api/Brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands([FromQuery] SearchForm request)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var brands = _context.Brands
                .Select(brand => new
                {
                    id = brand.Id,
                    name = brand.Name,
                    state = brand.State,
                    image = brand.Image,
                });
            //.ToListAsync();
            if (request.filter != null && request.keyword != null)
            {
                switch (request.filter)
                {
                    case "name":
                        brands = brands.Where(prod => prod.name.Contains(request.keyword));
                        break;
                    case "state":
                        brands = brands.Where(prod => prod.state.Contains(request.keyword));
                        break;
                    default:
                        break;
                }
            }
            var length = brands.Count();
            var records = await brands
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

        // GET: api/Brands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            if (_context.Brands == null)
            {
                return NotFound();
            }
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                code = 200,
                data = brand
            });
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
            await _context.SaveChangesAsync();
            return Ok(new
            {
                code = 200,
                message = "Success",
                data = item
            });
        }

        // POST: api/Brands
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Brand>> PostBrand([FromBody] NewBrandForm form)
        {
            if (_context.Brands == null)
            {
                return Problem("Entity set 'DbCtx.Brands'  is null.");
            }
            var brand = new Brand
            {
                Name = form.name,
                Image = form.image,
                State = BrandState.Active.ToString(),
            };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                code = 200,
                message = "Success",
                data = brand
            });
        }

        private bool BrandExists(int id)
        {
            return (_context.Brands?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
