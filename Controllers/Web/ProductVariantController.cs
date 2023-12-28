using asp.net.Data;
using asp.net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace asp.net.Controllers.Web
{
    [Route("v1/web/variants")]
    [ApiController]
    public class ProductVariantController : ControllerBase
    {
        private readonly DbCtx _context;

        public ProductVariantController(DbCtx context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariants>> Get(long id)
        {
            var variant = _context.ProductVariants
                .Where(v => v.Id == id)
                .Select(v => new
                {
                    v.Id,
                    v.Name,
                    sale_price = v.SalePrice,
                    standard_price = v.StandardPrice,
                    v.Discount,
                    v.Image,
                    v.State,
                    Product = new
                    {
                        v.Product.Id,
                        v.Product.Name,
                        v.Product.Image,
                        v.Product.Slug,
                        v.Product.State,
                    }
                })
                .First();
            if (variant == null)
            {
                return NotFound(new
                {
                    code = 404,
                    message = "Không tìm thấy sản phẩm"
                });
            }

            return Ok(new
            {
                code = 200,
                data = variant
            });
        }
    }
}
