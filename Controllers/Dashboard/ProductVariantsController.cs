﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        public class UpdatVariantForm
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



        //// POST: api/ProductVariants
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<ProductVariants>> PostProductVariants(ProductVariants productVariants)
        //{
        //    if (_context.ProductVariants == null)
        //    {
        //        return Problem("Entity set 'DbCtx.ProductVariants'  is null.");
        //    }
        //    _context.ProductVariants.Add(productVariants);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetProductVariants", new { id = productVariants.Id }, productVariants);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] UpdateVariantForm request)
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
        private bool ProductVariantsExists(int id)
        {
            return (_context.ProductVariants?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
