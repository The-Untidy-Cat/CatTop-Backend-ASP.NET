using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace asp.net.Controllers.Dashboard
{
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
    public class NewProductForm
    {
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("image")]
        public string Image { get; set; }

        [Required]
        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [Required]
        [JsonPropertyName("brand")]
        public int BrandId { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

    }
    public class SearchDateForStatistic
    {
        [JsonPropertyName("start_date")]
        [DataType(DataType.Date)]
        public string? start_date { get; set; }

        [JsonPropertyName("end_date")]
        [DataType(DataType.Date)]
        public string? end_date { get; set; }
    }

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
                    slug = prod.Slug,
                    created_at = prod.CreatedAt,
                    brand = new
                    {
                        id = prod.BrandId,
                        name = prod.Brand.Name
                    },
                    variant_count = prod.ProductVariants.Count()
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
            var product = await _context.Products.Include(p => p.ProductVariants)
                        .Where(p => p.Id == id).Select(obj => new
                        {
                            id = obj.Id,
                            name = obj.Name,
                            slug = obj.Slug,
                            description = obj.Description,
                            image = obj.Image,
                            state = obj.State,
                            variants = new
                            {
                                id = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().Id,
                                name = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().Name,
                                sku = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().SKU,
                                standard_price = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().StandardPrice,
                                sale_price = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().SalePrice,
                                discount = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().Discount,
                                state = obj.ProductVariants.Where(v => v.ProductID == obj.Id).FirstOrDefault().State,
                                sold = obj.ProductVariants.Select(v => v.OrderItems.Sum(v => v.Amount)).FirstOrDefault()
                            },
                            brand = new
                            {
                                id = obj.Brand.Id,
                                name = obj.Brand.Name,
                                image = obj.Brand.Image,
                                product_count = obj.Brand.Products.Count()
                            }
                        })
            .FirstOrDefaultAsync();
            if (product == null)
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
                data = product
            };

            return Ok(response);

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
                    variants = _context.ProductVariants.Where(v => v.ProductID == item.Id).Select(v => new
                    {
                        id = v.Id,
                        name = v.Name,
                        sku = v.SKU,
                        standard_price = v.StandardPrice,
                        sale_price = v.SalePrice,
                        discount = v.Discount,
                        state = v.State,
                        sold = v.OrderItems.Select(oi => oi.Amount).Sum()
                    }),

                    brands = _context.Brands.Where(b => b.Id == item.BrandId).Select(brand => new
                    {
                        id = brand.Id,
                        name = brand.Name,
                        image = brand.Image,
                        product_count = brand.Products.Count()
                    })
                }
            };
            return Ok(responseSuccess);
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Product>>> CreateProduct([FromBody] NewProductForm request)
        {

            var product = new Product
            {
                Name = request.Name,
                Slug = request.Slug,
                Image = request.Image,
                State = ProductState.Published.ToString(),
                CreatedAt = DateTime.Now,
                BrandId = request.BrandId,

            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var responseSuccess = new
            {
                code = 200,
                message = "Tạo sản phẩm thành công",
                data = new
                {
                    id = product.Id,
                    name = product.Name,
                    slug = product.Slug,
                    image = product.Image,
                    state = product.State,
                    brand_id = product.BrandId,
                }
            };

            return Ok(responseSuccess);
        }
        [HttpGet("statistics")]
        public async Task<ActionResult<IEnumerable<Product>>> GetStatisticsProducts([FromQuery] SearchDateForStatistic request)
        {

            var productsStatistic = new List<object>();
            var query = from v in _context.ProductVariants
                        join p in _context.Products on v.ProductID equals p.Id
                        join oi in _context.OrderItems on v.Id equals oi.VariantId
                        join od in _context.Orders on oi.OrderId equals od.Id
                        group new { p, v, oi, od } by new
                        {
                            p.Id,
                            p.Name,
                            VariantName = v.Name,
                            v.Discount,
                            v.SalePrice,
                            v.StandardPrice
                        } into obj
                        orderby obj.Sum(x => x.oi.Total) descending
                        select new
                        {
                            ProductId = obj.Key.Id,
                            ProductName = obj.Key.Name,
                            VariantName = obj.Key.VariantName,
                            OrderCount = obj.Count(),
                            TotalAmount = obj.Sum(x => x.oi.Amount),
                            TotalSum = obj.Sum(x => x.oi.Total),
                            Discount = obj.Key.Discount,
                            SalePrice = obj.Key.SalePrice,
                            StandardPrice = obj.Key.StandardPrice
                        };
            var result = query.ToList();


            if (request.start_date != null && request.end_date != null)
            {

                foreach (var total in result)
                {
                    var statistic = new
                    {
                        id = total.ProductId,
                        product_name = total.ProductName,
                        variant_name = total.VariantName,
                        total_order = total.OrderCount,
                        total_amount = total.TotalAmount,
                        total_sale = total.TotalSum,
                        discount = total.Discount,
                        sale_price = total.SalePrice,
                        standard_price = total.StandardPrice
                    };
                    productsStatistic.Add(statistic);
                }
            }

            var responseSuccess = new
            {
                code = 200,
                message = "Thống kê sản phẩm thành công",
                data = new
                {
                    productsStatistic,
                }
            };
            return Ok(responseSuccess);
        }
    }
}
