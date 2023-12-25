using asp.net.Data;
using asp.net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using NuGet.Protocol;
using System.ComponentModel;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace asp.net.Controllers.Web
{
    public class SearchForm
    {
        [FromQuery(Name = "name")]
        public string? Name { get; set; }

        [FromQuery(Name = "brand")]
        public string? Brand { get; set; }

        [FromQuery(Name = "min_price")]
        public long? MinPrice { get; set; }

        [FromQuery(Name = "max_price")]
        public long? MaxPrice { get; set; }

        [FromQuery(Name = "offset")]
        [IntegerValidator(MinValue = 0)]
        [DefaultValue(0)]
        public int Offset { get; set; }

        [FromQuery(Name = "limit")]
        [IntegerValidator(MinValue = 0)]
        [DefaultValue(5)]
        public int Limit { get; set; }
    }

    [Route("v1/web")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DbCtx _context;

        public ProductController(DbCtx context)
        {
            _context = context;
        }


        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> Search([FromQuery] SearchForm form)
        {
            var query = _context.Products.Include(p => p.ProductVariants).Include(p => p.Brand)
                .Where(query => query.State == ProductState.Published.ToString())
                .Where(query => query.ProductVariants.Any(v => v.State == VariantState.Published.ToString()));
            if (form.Name != null)
            {
                query = query.Where(p => p.Name.Contains(form.Name));
            }
            if (form.Brand != null)
            {
                query = query.Where(p => p.Brand.Name == form.Brand);
            }
            if (form.MinPrice != null)
            {
                query = query.Where(p => p.SalePrice >= form.MinPrice);
            }
            if (form.MaxPrice != null)
            {
                query = query.Where(p => p.SalePrice <= form.MaxPrice);
            }
            query = query.Distinct();
            int length = query.Count();
            query = query.Skip(form.Offset).Take(form.Limit);
            var records = await query.Select(p => new
            {
                p.Id,
                p.Name,
                p.Image,
                created_at = p.CreatedAt,
                p.Slug,
                discount = p.ProductVariants.Where(v => v.SalePrice == p.ProductVariants.Min(v => v.SalePrice)).FirstOrDefault().Discount,
                sale_price = p.ProductVariants.Min(v => v.SalePrice),
                standard_price = p.ProductVariants.Where(v => v.SalePrice == p.ProductVariants.Min(v => v.SalePrice)).FirstOrDefault().StandardPrice,
                variants = p.ProductVariants.Select(v => new
                {
                    v.Id,
                    cpu = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Cpu.ToString(),
                    ram = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Ram.ToString(),
                    storage = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Storage.ToString(),
                    display = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Display.ToString(),
                    card = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Gpu.ToString()
                }).ToList()
            }).ToListAsync();
            var response = new
            {
                code = 200,
                data = new
                {
                    records,
                    length,
                    form.Offset,
                    form.Limit
                }
            };
            return Ok(response);
        }

        [HttpGet("products/{slug}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductDetail(string slug)
        {
            var product = await _context.Products.Include(p => p.ProductVariants).Include(p => p.Brand)
                .Where(p => p.Slug == slug)
                .Where(p => p.State == ProductState.Published.ToString())
                .Where(p => p.ProductVariants.Any(v => v.State == VariantState.Published.ToString()))
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Image,
                    created_at = p.CreatedAt,
                    p.Slug,
                    p.State,
                    discount = p.ProductVariants.Where(v => v.SalePrice == p.ProductVariants.Min(v => v.SalePrice)).FirstOrDefault().Discount,
                    sale_price = p.ProductVariants.Min(v => v.SalePrice),
                    standard_price = p.ProductVariants.Where(v => v.SalePrice == p.ProductVariants.Min(v => v.SalePrice)).FirstOrDefault().StandardPrice,
                    brand = new
                    {
                        p.Brand.Name,
                        p.Brand.Image
                    },
                    variants = p.ProductVariants
                    .Select(v => new
                    {
                        v.Id,
                        v.Name,
                        v.Image,
                        v.SKU,
                        v.State,
                        standard_price = v.StandardPrice,
                        sale_price = v.SalePrice,
                        v.Discount,
                        extra_fee = v.ExtraFee,
                        tax_rate = v.TaxRate,
                        specifications = JsonConvert.DeserializeObject<Specifications>(v.Specifications),
                        reviews = v.OrderItems.Where(oi => oi.Rating != null).Select(oi => new
                        {
                            oi.Id,
                            oi.Rating,
                            oi.Review,
                            created_at = oi.CreatedAt,
                            updated_at = oi.UpdatedAt,
                            customer = new
                            {
                                first_name = oi.Order.Customer.FirstName,
                                last_name = oi.Order.Customer.LastName
                            }
                        }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync();
            if (product == null)
                return NotFound(new
                {
                    code = 404,
                    message = "Không tìm thấy sản phẩm/không khả dụng"
                });
            return Ok(new
            {
                code = 200,
                data = product
            });
        }
    }
}
