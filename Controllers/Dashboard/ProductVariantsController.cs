
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace asp.net.Controllers.Dashboard
{
    public class UpdateDetailVariantForm
    {
        [JsonPropertyName("sku")]
        public string? SKU { get; set; }
        [JsonPropertyName("image")]
        [Url]
        public string? Image { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("standard_price")]
        public long StandardPrice { get; set; }
        [JsonPropertyName("tax_rate")]
        public double TaxRate { get; set; }
        [JsonPropertyName("discount")]
        public double Discount { get; set; }
        [JsonPropertyName("extra_fee")]
        public long ExtraFee { get; set; }
        [JsonPropertyName("cost_price")]
        public long CostPrice { get; set; }
        [Required]
        [JsonPropertyName("specifications")]
        public Specifications Specifications { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }
    }
    public class NewVariantForm
    {
        [Required]
        [JsonPropertyName("sku")]
        public string? SKU { get; set; }
        [Required]
        [JsonPropertyName("image")]
        public string? Image { get; set; }
        [Required]
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [Required]
        [JsonPropertyName("standard_price")]
        public long StandardPrice { get; set; }
        [Required]
        [JsonPropertyName("tax_rate")]
        public double TaxRate { get; set; }
        [Required]
        [JsonPropertyName("discount")]
        public double Discount { get; set; }
        [Required]
        [JsonPropertyName("extra_fee")]
        public long ExtraFee { get; set; }
        [Required]
        [JsonPropertyName("cost_price")]
        public long CostPrice { get; set; }
        [Required]
        [JsonPropertyName("specifications")]
        public Specifications Specifications { get; set; }

        [Required]
        [JsonPropertyName("state")]
        public string? State { get; set; }
    }
    [Route("v1/dashboard")]
    [ApiController]
    public class ProductVariantsController : ControllerBase
    {
        private readonly DbCtx _context;

        public ProductVariantsController(DbCtx context)
        {
            _context = context;
        }

        [HttpGet("variants")]
        public async Task<ActionResult<IEnumerable<ProductVariants>>> GetProductVariants([FromQuery] SearchForm form)
        {
            var variants = _context.ProductVariants
                .Where(v => v.State == VariantState.Published.ToString() && v.Product.State == ProductState.Published.ToString())
                .Select(v => new
                {
                    id = v.Id,
                    sku = v.SKU,
                    image = v.Image,
                    name = v.Name,
                    standard_price = v.StandardPrice,
                    tax_rate = v.TaxRate,
                    discount = v.Discount,
                    extra_fee = v.ExtraFee,
                    cost_price = v.CostPrice,
                    sale_price = v.SalePrice,
                    product = new
                    {
                        id = v.Product.Id,
                        name = v.Product.Name,
                        brand = v.Product.Brand.Name,
                    },
                });
            if (form.filter != null && form.keyword != null)
            {
                switch (form.filter)
                {
                    case "id":
                        variants = variants.Where(v => v.id.ToString() == form.keyword);
                        break;
                    case "name":
                        variants = variants.Where(v => v.name.Contains(form.keyword));
                        break;
                    case "SKU":
                        variants = variants.Where(v => v.sku.Contains(form.keyword));
                        break;
                    case "product":
                        variants = variants.Where(v => v.product.name.Contains(form.keyword));
                        break;
                    case "brand":
                        variants = variants.Where(v => v.product.brand.Contains(form.keyword));
                        break;
                }
            }
            var length = await variants.CountAsync();
            var records = await variants
                .Skip(form.offset)
                .Take(form.limit)
                .ToListAsync();
            return Ok(new
            {
                code = 200,
                data = new
                {
                    records,
                    length,
                    form.limit,
                    form.offset
                }
            });
        }

        [HttpPost("products/{productId}/variants")]
        public async Task<ActionResult<ProductVariants>> CreateProductVariant(int productId, [FromBody] NewVariantForm request)
        {
            var product = await _context.Products
                .Where(p => p.Id == productId)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy sản phẩm"
                };
                return NotFound(response);
            }

            var variant = new ProductVariants
            {
                SKU = request.SKU,
                Image = request.Image,
                Name = request.Name,
                StandardPrice = request.StandardPrice,
                TaxRate = request.TaxRate,
                Discount = request.Discount,
                ExtraFee = request.ExtraFee,
                CostPrice = request.CostPrice,
                SalePrice = request.StandardPrice + request.ExtraFee - (long)(request.Discount * request.StandardPrice),
                Specifications = JsonConvert.SerializeObject(request.Specifications),
                State = request.State,
                ProductID = productId,
                Created_at = DateTime.Now,
                Updated_at = DateTime.Now
            };

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            var responseSuccess = new
            {
                code = 200,
                message = "Thêm biến thể thành công",
                data = new
                {
                    id = variant.Id,
                    SKU = variant.SKU,
                    image = variant.Image,
                    name = variant.Name,
                    standard_price = variant.StandardPrice,
                    tax_rate = variant.TaxRate,
                    discount = variant.Discount,
                    extra_fee = variant.ExtraFee,
                    cost_price = variant.CostPrice,
                    specification = new
                    {
                        cpu = new
                        {
                            name = request.Specifications.Cpu.Name,
                            cores = request.Specifications.Cpu.Cores,
                            threads = request.Specifications.Cpu.Threads,
                            base_clock = request.Specifications.Cpu.BaseClock,
                            turbo_clock = request.Specifications.Cpu.TurboClock,
                            cache = request.Specifications.Cpu.Cache
                        },
                        ram = new
                        {
                            capacity = request.Specifications.Ram.Capacity,
                            type = request.Specifications.Ram.Type,
                            frequency = request.Specifications.Ram.Frequency,
                        },
                        storage = new
                        {
                            drive = request.Specifications.Storage.Drive.ToString(),
                            capacity = request.Specifications.Storage.Capacity.ToString(),
                            type = request.Specifications.Storage.Type.ToString(),
                        },
                        display = new
                        {
                            size = request.Specifications.Display.Size.ToString(),
                            resolution = request.Specifications.Display.Resolution.ToString(),
                        }
                    }
                }
            };
            return Ok(responseSuccess);
        }

        // GET: api/ProductVariants/5
        [HttpGet("products/{productId}/variants/{id}")]
        public async Task<ActionResult<ProductVariants>> GetProductVariants(int id)
        {
            if (_context.ProductVariants == null)
            {
                return NotFound();
            }
            var variants = await _context.ProductVariants
                .Where(v => v.Id == id)
                .Select(v => new
                {
                    id = v.Id,
                    SKU = v.SKU,
                    image = v.Image,
                    name = v.Name,
                    standard_price = v.StandardPrice,
                    tax_rate = v.TaxRate,
                    discount = v.Discount,
                    extra_fee = v.ExtraFee,
                    cost_price = v.CostPrice,
                    specification = JsonConvert.DeserializeObject<Specifications>(v.Specifications)

                }).FirstOrDefaultAsync();
            var response = new
            {
                code = 200,
                data =
                    variants

            };
            return Ok(response);
        }

        [HttpPut("products/{productId}/variants/{id}")]
        public async Task<IActionResult> UpdateProductVariantDetail(int pid, int vid, [FromBody] UpdateDetailVariantForm request)
        {
            var product = await _context.Products
                .Where(p => p.Id == pid)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy sản phẩm"
                };
                return NotFound(response);
            }

            var variant = await _context.ProductVariants
                .Where(v => v.Id == vid)
                .FirstOrDefaultAsync();
            if (variant == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy biến thể"
                };
                return NotFound(response);
            }

            if (request.Name != null)
            {
                variant.Name = request.Name;
            }
            if (request.SKU != null)
            {
                variant.SKU = request.SKU;
            }

            if (request.Image != null)
            {
                variant.Image = request.Image;
            }
            if (request.State != null)
            {
                variant.State = request.State;
            }
            if (request.StandardPrice != 0)
            {
                variant.StandardPrice = request.StandardPrice;
            }
            if (request.TaxRate != 0)
            {
                variant.TaxRate = request.TaxRate;
            }
            if (request.Discount != 0)
            {
                variant.Discount = request.Discount;
            }
            if (request.ExtraFee != 0)
            {
                variant.ExtraFee = request.ExtraFee;
            }
            if (request.CostPrice != 0)
            {
                variant.CostPrice = request.CostPrice;
            }

            variant.Specifications = JsonConvert.SerializeObject(request.Specifications);

            variant.Updated_at = DateTime.Now;
            await _context.SaveChangesAsync();

            var responseSuccess = new
            {
                code = 200,
                message = "Cập nhật biến thể thành công",
                data = new
                {
                    id = variant.Id,
                    sku = variant.SKU,
                    name = variant.Name,
                    standard_price = variant.StandardPrice,
                    tax_rate = variant.TaxRate,
                    discount = variant.Discount,
                    extra_fee = variant.ExtraFee,
                    cost_price = variant.CostPrice,
                    specifications = new
                    {
                        cpu = new
                        {
                            name = request.Specifications.Cpu.Name.ToString(),
                            cores = request.Specifications.Cpu.Cores,
                            threads = request.Specifications.Cpu.Threads,
                            base_clock = request.Specifications.Cpu.BaseClock,
                            turbo_clock = request.Specifications.Cpu.TurboClock,
                            cache = request.Specifications.Cpu.Cache,
                        },
                        ram = new
                        {
                            capacity = request.Specifications.Ram.Capacity,
                            type = request.Specifications.Ram.Type.ToString(),
                            frequency = request.Specifications.Ram.Frequency.ToString(),
                        },
                        storage = new
                        {
                            drive = request.Specifications.Storage.Drive.ToString(),
                            capacity = request.Specifications.Storage.Capacity.ToString(),
                            type = request.Specifications.Storage.Type.ToString(),
                        },
                        display = new
                        {
                            size = request.Specifications.Display.Size.ToString(),
                            resolution = request.Specifications.Display.Resolution.ToString(),
                            technology = request.Specifications.Display.Technology.ToString(),
                            refresh_rate = request.Specifications.Display.RefreshRate,
                            touch = request.Specifications.Display.Touch.ToString(),
                        },
                        gpu = new
                        {
                            name = request.Specifications.Gpu.Name.ToString(),
                            memory = request.Specifications.Gpu.Memory.ToString(),
                            type = request.Specifications.Gpu.Type.ToString(),
                            frequency = request.Specifications.Gpu.Frequency,
                        },
                        ports = request.Specifications.Ports,
                        keyboard = request.Specifications.Keyboard,
                        touchpad = request.Specifications.Touchpad,
                        webcam = request.Specifications.Webcam,
                        battery = request.Specifications.Battery,
                        weight = request.Specifications.Weight,
                        os = request.Specifications.Os,
                        warranty = request.Specifications.Warranty,
                        color = request.Specifications.Color,
                    },
                    state = variant.State,
                    sold = _context.OrderItems.Where(oi => oi.VariantId == vid).Sum(oi => oi.Amount)
                }
            };
            return Ok(responseSuccess);
        }
    }
}
