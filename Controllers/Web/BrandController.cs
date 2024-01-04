using asp.net.Data;
using asp.net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace asp.net.Controllers.Web
{
    [Route("v1/web/brands")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly DbCtx _context;

        public BrandController(DbCtx context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> Get()
        {
            var brands = _context.Brands
                .Where(b => b.State == "active")
                .Select(b => new
                {
                    id = b.Id,
                    name = b.Name,
                    image = b.Image,
                    product_count = b.Products.Count(p => p.State == "published"),
                })
                .ToList();
            return Ok(new
            {
                code = 200,
                data = brands,
            });
        }
    }
}
