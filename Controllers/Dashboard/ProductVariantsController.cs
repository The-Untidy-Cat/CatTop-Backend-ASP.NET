
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
    [Route("v1/dashboard/product_variants")]
    [ApiController]
    public class ProductVariantsController : ControllerBase
    {
        private readonly DbCtx _context;

        public ProductVariantsController(DbCtx context)
        {
            _context = context;
        }

        public class UpdateVariantForm
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("sku")]
            public string? Sku { get; set; }

            [JsonPropertyName("image")]
            [Url]
            public string? Image { get; set; }

            [JsonPropertyName("standard_price")]
            public long StandardPrice { get; set; }

            [JsonPropertyName("tax_rate")]
            public double TaxRate { get; set; }

            [JsonPropertyName("discount")]
            public double Discount { get; set; }

            [JsonPropertyName("extra_fee")]
            public double ExtraFee { get; set; }

            [JsonPropertyName("cost_price")]
            public long CostPrice { get; set; }

            [JsonPropertyName("sale_price")]
            public long SalePrice { get; set; }
           
            [JsonPropertyName("cpu")]
            public Cpu? Cpu { get; set; }

            [JsonPropertyName("ram")]
            public Ram? Ram { get; set; }

            [JsonPropertyName("storage")]
            public Storage? Storage { get; set; }

            [JsonPropertyName("display")]
            public Display? Display { get; set; }

            [JsonPropertyName("gpu")]
            public Gpu? Gpu { get; set; }

            [JsonPropertyName("ports")]
            public string? Ports { get; set; }

            [JsonPropertyName("keyboard")]
            public string? Keyboard { get; set; }

            [JsonPropertyName("touchpad")]
            public bool? Touchpad { get; set; }

            [JsonPropertyName("webcam")]
            public bool? Webcam { get; set; }

            [JsonPropertyName("battery")]
            public int? Battery { get; set; }

            [JsonPropertyName("weight")]
            public double? Weight { get; set; }

            [JsonPropertyName("os")]
            public string? Os { get; set; }

            [JsonPropertyName("warranty")]
            public int? Warranty { get; set; }

            [JsonPropertyName("color")]
            public string? Color { get; set; }

        }
        public class NewVariantForm
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("sku")]
            public string? Sku { get; set; }

            [JsonPropertyName("image")]
            [Url]
            public string? Image { get; set; }

            [JsonPropertyName("standard_price")]
            public long StandardPrice { get; set; }

            [JsonPropertyName("tax_rate")]
            public double TaxRate { get; set; }

            [JsonPropertyName("discount")]
            public double Discount { get; set; }

            [JsonPropertyName("extra_fee")]
            public double ExtraFee { get; set; }

            [JsonPropertyName("cost_price")]
            public long CostPrice { get; set; }

            [JsonPropertyName("sale_price")]
            public long SalePrice { get; set; }
            [JsonPropertyName("product_id")]
            public long ProductId { get; set; }

            //Specifications
            [JsonPropertyName("cpu")]
            public Cpu? Cpu { get; set; }

            [JsonPropertyName("ram")]
            public Ram? Ram { get; set; }

            [JsonPropertyName("storage")]
            public Storage? Storage { get; set; }

            [JsonPropertyName("display")]
            public Display? Display { get; set; }

            [JsonPropertyName("gpu")]
            public Gpu? Gpu { get; set; }

            [JsonPropertyName("ports")]
            public string? Ports { get; set; }

            [JsonPropertyName("keyboard")]
            public string? Keyboard { get; set; }

            [JsonPropertyName("touchpad")]
            public bool? Touchpad { get; set; }

            [JsonPropertyName("webcam")]
            public bool? Webcam { get; set; }

            [JsonPropertyName("battery")]
            public int? Battery { get; set; }

            [JsonPropertyName("weight")]
            public double? Weight { get; set; }

            [JsonPropertyName("os")]
            public string? Os { get; set; }

            [JsonPropertyName("warranty")]
            public int? Warranty { get; set; }

            [JsonPropertyName("color")]
            public string? Color { get; set; }

        }
        // GET: api/ProductVariants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariants>> GetProductVariants(int id)
        {
            if (_context.ProductVariants == null)
            {
                return NotFound();
            }
            var variants = _context.ProductVariants
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
                    specification = new
                    {
                        cpu = new
                        {
                            name = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Cpu.Name.ToString(),
                            cores = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(v.Specifications).Cpu.Cores),
                            threads = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(v.Specifications).Cpu.Threads),
                            base_clock = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(v.Specifications).Cpu.BaseClock),
                            turbo_clock = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(v.Specifications).Cpu.TurboClock),
                            cache = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(v.Specifications).Cpu.Cache),
                        },
                        ram = new
                        {
                            capacity = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(v.Specifications).Ram.Capacity),
                            type = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Ram.Type.ToString(),
                            frequency = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(v.Specifications).Ram.Frequency.ToString()),
                        },
                        storage = new
                        {
                            drive = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Storage.Drive.ToString(),
                            capacity = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Storage.Capacity.ToString(),
                            type = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Storage.Type.ToString(),
                        },
                        display = new
                        {
                            size = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Display.Size.ToString(),
                            resolution = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Display.Resolution.ToString(),
                            technology = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Display.Technology.ToString(),
                            refresh_rate = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(v.Specifications).Display.RefreshRate),
                            touch = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Display.Touch.ToString(),
                        },
                        gpu = new
                        {
                            name = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Gpu.Name.ToString(),
                            memory = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Gpu.Memory.ToString(),
                            type = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Gpu.Type.ToString(),
                            frequency = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(v.Specifications).Gpu.Frequency),
                        },
                        ports = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Ports,
                        keyboard = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Keyboard,
                        touchpad = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Touchpad,
                        webcam = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Webcam,
                        battery = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Battery,
                        weight = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Weight,
                        os = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Os,
                        warranty = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Warranty,
                        color = JsonConvert.DeserializeObject<Specifications>(v.Specifications).Color,
                    },
                    state = v.State,
                    sold = v.OrderItems.Sum(oi => oi.Amount)

                });


            var response = new
            {
                code = 200,
                data = new
                {
                    variants
                }
            };
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductVariant(int id, [FromBody] UpdateVariantForm request)
        {
            var item = await _context.ProductVariants
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
            if (item == null)
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
                item.Name = request.Name;
            }
            if (request.Sku != null)
            {
                item.SKU = request.Sku;
            }
            if (request.Image != null)
            {
                item.Image = request.Image;
            }
            if (request.StandardPrice != 0)
            {
                item.StandardPrice = request.StandardPrice;
            }
            if (request.SalePrice != 0)
            {
                item.SalePrice = request.SalePrice;
            }
            if (request.CostPrice != 0)
            {
                item.CostPrice = request.CostPrice;
            }
            if (request.TaxRate != 0)
            {
                item.TaxRate = request.TaxRate;
            }
            if (request.Discount != 0)
            {
                item.Discount = request.Discount;
            }

            item.Specifications = "{\"cpu\":{\"name\":\"" + request.Cpu.Name + "\",\"cores\":\"" + request.Cpu.Cores + "\",\"threads\":\"" + request.Cpu.Threads + "\",\"base_clock\":\"" + request.Cpu.BaseClock + "\",\"turbo_clock\":\"" + request.Cpu.TurboClock + "\",\"cache\":\"" + request.Cpu.Cache + "\"},\"ram\":{\"capacity\":\"" + request.Ram.Capacity + "\",\"type\":\"" + request.Ram.Type + "\",\"frequency\":\"" + request.Ram.Frequency + "\"},\"storage\":{\"drive\":\"" + request.Storage.Drive + "\",\"capacity\":\"" + request.Storage.Capacity + "\",\"type\":\"" + request.Storage.Type + "\"},\"display\":{\"size\":\"" + request.Display.Size + "\",\"resolution\":\"" + request.Display.Resolution + "\",\"technology\":\"" + request.Display.Technology + "\",\"refresh_rate\":\"" + request.Display.RefreshRate + "\",\"touch\":\"" + request.Display.Touch + "\"},\"gpu\":{\"name\":\"" + request.Gpu.Name + "\",\"memory\":\"" + request.Gpu.Memory + "\",\"type\":\"" + request.Gpu.Type + "\",\"frequency\":\"" + request.Gpu.Frequency + "\"},\"ports\":\"" + request.Ports + "\",\"keyboard\":\"" + request.Keyboard + "\",\"touchpad\":\"" + request.Touchpad + "\",\"webcam\":\"" + request.Webcam + "\",\"battery\":\"" + request.Battery + "\",\"weight\":\"" + request.Weight + "\",\"os\":\"" + request.Os + "\",\"warranty\":\"" + request.Warranty + "\",\"color\":\"" + request.Color + "\"}";

            item.Updated_at = DateTime.Now;
            await _context.SaveChangesAsync();

            var responseSuccess = new
            {
                code = 200,
                message = "Cập nhật biến thể thành công",
                data = new
                {
                    id = item.Id,
                    SKU = item.SKU,
                    image = item.Image,
                    name = item.Name,
                    standard_price = item.StandardPrice,
                    tax_rate = item.TaxRate,
                    discount = item.Discount,
                    extra_fee = item.ExtraFee,
                    cost_price = item.CostPrice,
                    specification = new
                    {
                        cpu = new
                        {
                            name = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Cpu.Name.ToString(),
                            cores = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(item.Specifications).Cpu.Cores),
                            threads = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(item.Specifications).Cpu.Threads),
                            base_clock = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(item.Specifications).Cpu.BaseClock),
                            turbo_clock = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(item.Specifications).Cpu.TurboClock),
                            cache = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(item.Specifications).Cpu.Cache),
                        },
                        ram = new
                        {
                            capacity = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(item.Specifications).Ram.Capacity),
                            type = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Ram.Type.ToString(),
                            frequency = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(item.Specifications).Ram.Frequency.ToString()),
                        },
                        storage = new
                        {
                            drive = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Storage.Drive.ToString(),
                            capacity = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Storage.Capacity.ToString(),
                            type = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Storage.Type.ToString(),
                        },
                        display = new
                        {
                            size = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Display.Size.ToString(),
                            //resolution = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Display.Resolution.ToString(),
                            technology = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Display.Technology.ToString(),
                            refresh_rate = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(item.Specifications).Display.RefreshRate),
                            touch = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Display.Touch.ToString(),
                        },
                        gpu = new
                        {
                            name = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Gpu.Name.ToString(),
                            memory = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Gpu.Memory.ToString(),
                            type = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Gpu.Type.ToString(),
                            frequency = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(item.Specifications).Gpu.Frequency),
                        },
                        ports = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Ports,
                        keyboard = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Keyboard,
                        touchpad = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Touchpad,
                        webcam = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Webcam,
                        battery = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Battery,
                        weight = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Weight,
                        os = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Os,
                        warranty = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Warranty,
                        color = JsonConvert.DeserializeObject<Specifications>(item.Specifications).Color,
                    }
                }
            };
            return Ok(responseSuccess);
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProductVariants>>> CreateProductVariant([FromBody] NewVariantForm request)
        {
            var existedProduct = await _context.Products
                    .Where(p => p.Id == request.ProductId)
                    .FirstOrDefaultAsync();
            if (existedProduct == null)
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
                Name = request.Name,
                SKU = request.Sku,
                StandardPrice = request.StandardPrice,
                TaxRate = request.TaxRate,
                Discount = request.Discount,
                ExtraFee = request.ExtraFee,
                CostPrice = request.CostPrice,
                Specifications = "{\"cpu\":{\"name\":\"" + request.Cpu.Name + "\",\"cores\":\"" + request.Cpu.Cores + "\",\"threads\":\"" + request.Cpu.Threads + "\",\"base_clock\":\"" + request.Cpu.BaseClock + "\",\"turbo_clock\":\"" + request.Cpu.TurboClock + "\",\"cache\":\"" + request.Cpu.Cache + "\"},\"ram\":{\"capacity\":\"" + request.Ram.Capacity + "\",\"type\":\"" + request.Ram.Type + "\",\"frequency\":\"" + request.Ram.Frequency + "\"},\"storage\":{\"drive\":\"" + request.Storage.Drive + "\",\"capacity\":\"" + request.Storage.Capacity + "\",\"type\":\"" + request.Storage.Type + "\"},\"display\":{\"size\":\"" + request.Display.Size + "\",\"resolution\":\"" + request.Display.Resolution + "\",\"technology\":\"" + request.Display.Technology + "\",\"refresh_rate\":\"" + request.Display.RefreshRate + "\",\"touch\":\"" + request.Display.Touch + "\"},\"gpu\":{\"name\":\"" + request.Gpu.Name + "\",\"memory\":\"" + request.Gpu.Memory + "\",\"type\":\"" + request.Gpu.Type + "\",\"frequency\":\"" + request.Gpu.Frequency + "\"},\"ports\":\"" + request.Ports + "\",\"keyboard\":\"" + request.Keyboard + "\",\"touchpad\":\"" + request.Touchpad + "\",\"webcam\":\"" + request.Webcam + "\",\"battery\":\"" + request.Battery + "\",\"weight\":\"" + request.Weight + "\",\"os\":\"" + request.Os + "\",\"warranty\":\"" + request.Warranty + "\",\"color\":\"" + request.Color + "\"}",
                ProductID = request.ProductId,
                State = VariantState.Published.ToString(),
                Created_at = DateTime.Now,
            };
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            var responseSuccess = new
            {
                code = 200,
                message = "Tạo biến thể thành công",
                data = new
                {
                    id = variant.Id,
                    SKU = variant.SKU,
                    //image = variant.Image,
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
                            name = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Cpu.Name.ToString(),
                            cores = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Cpu.Cores),
                            threads = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Cpu.Threads),
                            base_clock = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Cpu.BaseClock),
                            turbo_clock = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Cpu.TurboClock),
                            cache = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Cpu.Cache),
                        },
                        ram = new
                        {
                            capacity = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Ram.Capacity),
                            type = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Ram.Type.ToString(),
                            frequency = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Ram.Frequency.ToString()),
                        },
                        storage = new
                        {
                            drive = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Storage.Drive.ToString(),
                            capacity = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Storage.Capacity.ToString(),
                            type = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Storage.Type.ToString(),
                        },
                        display = new
                        {
                            size = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Display.Size.ToString(),
                            resolution = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Display.Resolution.ToString(),
                            technology = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Display.Technology.ToString(),
                            refresh_rate = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Display.RefreshRate),
                            touch = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Display.Touch.ToString(),
                        },
                        gpu = new
                        {
                            name = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Gpu.Name.ToString(),
                            memory = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Gpu.Memory.ToString(),
                            type = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Gpu.Type.ToString(),
                            frequency = Convert.ToInt32(JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Gpu.Frequency),
                        },
                        ports = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Ports,
                        keyboard = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Keyboard,
                        touchpad = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Touchpad,
                        webcam = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Webcam,
                        battery = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Battery,
                        weight = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Weight,
                        os = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Os,
                        warranty = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Warranty,
                        color = JsonConvert.DeserializeObject<Specifications>(variant.Specifications).Color,
                    },
                    state = variant.State,
                    sale_price = variant.SalePrice
                }
        };

            return Ok(responseSuccess);
        }
    }
}
