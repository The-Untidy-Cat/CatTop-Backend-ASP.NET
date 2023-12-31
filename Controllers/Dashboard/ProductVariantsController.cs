
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

        //public class Specifications
        //{
        //    public Cpu? Cpu { get; set; }
        //    public Ram? Ram { get; set; }
        //    public Storage? Storage { get; set; }
        //    public Display? Display { get; set; }
        //    public Gpu? Gpu { get; set; }
        //    public string? Ports { get; set; }
        //    public string? Keyboard { get; set; }
        //    public bool? Touchpad { get; set; }
        //    public bool? Webcam { get; set; }
        //    public int? Battery { get; set; }
        //    public double? Weight { get; set; }
        //    public string? Os { get; set; }
        //    public int? Warranty { get; set; }
        //    public string? Color { get; set; }
        //}
        public class UpdateVariantForm
        {
            //[JsonPropertyName("name")]
            //public string? Name { get; set; }

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

            //if (request.Name != null)
            //{
            //    item.Name = request.Name;
            //}

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
                            name = request.Cpu.Name,
                            cores = request.Cpu.Cores,
                            threads = request.Cpu.Threads,
                            base_clock = request.Cpu.BaseClock,
                            turbo_clock = request.Cpu.TurboClock,
                            cache = request.Cpu.Cache
                        },
                        ram = new
                        {
                            capacity = request.Ram.Capacity,
                            type = request.Ram.Type,
                            frequency = request.Ram.Frequency,
                        },
                        storage = new
                        {
                            drive = request.Storage.Drive.ToString(),
                            capacity = request.Storage.Capacity.ToString(),
                            type = request.Storage.Type.ToString(),
                        },
                        display = new
                        {
                            size = request.Display.Size.ToString(),
                            resolution = request.Display.Resolution.ToString(),
                            technology = request.Display.Technology.ToString(),
                            refresh_rate = request.Display.RefreshRate.ToString(),
                            touch = request.Display.Touch.ToString(),
                        },
                        gpu = new
                        {
                            name = request.Gpu.Name.ToString(),
                            memory = request.Gpu.Memory.ToString(),
                            type = request.Gpu.Type.ToString(),
                            frequency = request.Gpu.Frequency.ToString(),
                        },
                        ports = request.Ports.ToString(),
                        keyboard = request.Keyboard.ToString(),
                        touchpad = request.Touchpad.ToString(),
                        webcam = request.Webcam.ToString(),
                        battery = request.Battery.ToString(),
                        weight = request.Weight.ToString(),
                        os = request.Os.ToString(),
                        warranty = request.Warranty.ToString(),
                        color = request.Color.ToString(),
                    },
                }
                    //state = item.State,
                    //sold = item.OrderItems.Sum(oi => oi.Amount)
            };
            return Ok(responseSuccess);
        }
    }
}
