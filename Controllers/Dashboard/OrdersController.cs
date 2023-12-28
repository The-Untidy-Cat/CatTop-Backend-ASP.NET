using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace asp.net.Controllers.Dashboard
{
  
    public class NewOrderForm
    {
        [Required]
        [JsonPropertyName("customer_id")]
        public int CustomerId { get; set; }

        [Required]
        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; }

        public List<NewOrderItem>? Items { get; set; }
    }

    public class NewOrderItem
    {
        [Required]
        [JsonPropertyName("variant_id")]
        public long VariantId { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
    }

    [Route("v1/dashboard/orders")]
    [ApiController]

    public class OrdersController : ControllerBase
    {
        private readonly DbCtx _context;

        public OrdersController(DbCtx context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders([FromQuery] SearchForm request)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }

            var orders = _context.Orders
                .Select(o => new
                {
                    id = o.Id,
                    customer = new
                    {
                        customer_id = o.CustomerId,
                        first_name = o.Customer.FirstName,
                        last_name = o.Customer.LastName,
                    },
                    employee = new
                    {
                        employee_id = o.EmployeeId,
                        first_name = o.Employee.FirstName,
                        last_name = o.Employee.LastName,
                    },
                    items = o.OrderItems.Where(OrderItems => OrderItems.OrderId == o.Id).Select(i => new
                    {
                        order_id = i.OrderId,
                        variant_id = i.VariantId,
                        amount = i.Amount,
                        sale_price = i.SalePrice,
                        variant = new
                        {
                            id = i.ProductVariant.Id,
                            SKU = i.ProductVariant.SKU,
                            name = i.ProductVariant.Name,
                            description = i.ProductVariant.Description,
                            standard_price = i.ProductVariant.StandardPrice,
                            tax_rate = i.ProductVariant.TaxRate,
                            discount = i.ProductVariant.Discount,
                            extra_fee = i.ProductVariant.ExtraFee,
                            cost_price = i.ProductVariant.CostPrice,
                            sale_price = i.ProductVariant.SalePrice,
                            specifications = new
                            {
                                cpu = new
                                {
                                    name = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Cpu.Name.ToString(),
                                    cores = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Cpu.Cores.ToString(),
                                    threads = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Cpu.Threads.ToString(),
                                    base_clock = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Cpu.BaseClock.ToString(),
                                    turbo_clock = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Cpu.TurboClock.ToString(),
                                    cache = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Cpu.Cache.ToString(),
                                },
                                ram = new
                                {
                                    capacity = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Ram.Capacity.ToString(),
                                    type = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Ram.Type.ToString(),
                                    frequency = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Ram.Frequency.ToString(),
                                },
                                storage = new
                                {
                                    drive = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Storage.Drive.ToString(),
                                    capacity = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Storage.Capacity.ToString(),
                                    type = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Storage.Type.ToString(),
                                },
                                display = new
                                {
                                    size = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Display.Size.ToString(),
                                    resolution = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Display.Resolution.ToString(),
                                    technology = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Display.Technology.ToString(),
                                    touch = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Display.Touch.ToString(),
                                },
                                gpu = new
                                {
                                    name = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Gpu.Name.ToString(),
                                    memory = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Gpu.Memory.ToString(),
                                    type = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Gpu.Type.ToString(),
                                    frequency = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Gpu.Frequency.ToString(),
                                },
                                ports = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Ports,
                                keyboard = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Keyboard,
                                touchpad = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Touchpad,
                                webcam = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Webcam,
                                battery = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Battery,
                                weight = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Weight,
                                os = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Os,
                                warranty = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Warranty,
                                color = JsonConvert.DeserializeObject<Specifications>(i.ProductVariant.Specifications).Color,
                            },
                            state = i.ProductVariant.State,
                            created_at = i.ProductVariant.Created_at,
                            updated_at = i.ProductVariant.Updated_at,
                            sold = i.Order.OrderItems.Where(i => i.OrderId == i.Id).Count(),
                            product = new
                            {
                                id = i.ProductVariant.Product.Id,
                                name = i.ProductVariant.Product.Name,
                                variant_count = i.ProductVariant.Product.ProductVariants.Count(),
                                discount = i.ProductVariant.Discount,
                                sale_price = i.ProductVariant.SalePrice,
                                standard_price = i.ProductVariant.StandardPrice,
                            }

                        }
                    }),


                }); ;
            

         
            if (request.filter != null && request.keyword != null)
            {
                switch (request.filter)
                {
                    case "customer_firstname":
                        orders = orders.Where(o => o.customer.first_name.ToString().Contains(request.keyword));
                        break;
                    case "employee_firstname":
                        orders = orders.Where(o => o.employee.first_name.ToString().Contains(request.keyword));
                        break;
                    default:
                        break;
                }
            }
                var length = orders.Count();
            var records =
                await orders
                .Skip(request.offset).Take(request.limit)
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
            //return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound(new
                {
                    code = 404,
                    message = "Lỗi Context"
                });
            }

            var order = _context.Orders
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    id = o.Id,
                    shopping_method = o.ShoppingMethod,
                    payment_method = o.PaymentMethod,
                    payment_state = o.PaymentState,
                    state = o.State,
                    note = o.Note,
                    created_at = o.CreatedAt,
                    updated_at = o.UpdatedAt,
                    total = o.OrderItems.Where(i => i.OrderId == i.Id).Sum(i => i.Total),
                    customer = new
                    {
                        id = o.CustomerId,
                        first_name = o.Customer.FirstName,
                        last_name = o.Customer.LastName,
                        order_count = o.Customer.Orders.Count(),
                    },
                    employee = new
                    {
                        id = o.EmployeeId,
                        first_name = o.Employee.FirstName,
                        last_name = o.Employee.LastName,
                        order_count = o.Employee.Orders.Count(),
                    },
                    items = o.OrderItems.Where(OrderItems => OrderItems.OrderId == o.Id).Select(i => new
                    {
                        id = i.Id,
                        variant_id = i.VariantId,
                        amount = i.Amount,
                        sale_price = i.SalePrice,
                        total = i.Total,
                        order_id = i.OrderId,
                        rating = i.Rating,
                        review = i.Review,
                        variant = new
                        {
                            id = i.ProductVariant.Id,
                            name = i.ProductVariant.Name,
                            sku = i.ProductVariant.SKU,
                            sold = i.ProductVariant.OrderItems.Count(),
                            product = new
                            {
                                id = i.ProductVariant.Product.Id,
                                name = i.ProductVariant.Product.Name,
                                variant_count = i.ProductVariant.Product.ProductVariants.Count(),
                                discount = i.ProductVariant.Discount,
                                sale_price = i.ProductVariant.SalePrice,
                                standard_price = i.ProductVariant.StandardPrice,
                            }
                        }
                        
                    }).ToList(),
                });
        
            if (order == null)
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
                data = new
                {
                    order
                }
            };    

            return Ok(response);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] NewOrderForm request)
        {
            if(!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "fail in PostOrder in dash",
                    errors = ModelState.Values.SelectMany(t => t.Errors.Select(a => a.ErrorMessage))
                };
                return BadRequest(response);
            }
            var user = HttpContext.Items["user"];
            var employee = _context.Employees.Where(e => e.User.Username == user).FirstOrDefaultAsync();
            var customer = _context.Customers.Where(c => c.Id == request.CustomerId).FirstOrDefaultAsync();
            var order = new Order
            {
                CustomerId = customer.Id,
                EmployeeId = employee.Id,
                PaymentMethod = request.PaymentMethod,
                ShoppingMethod = ShoppingMethod.Online.ToString(),
                PaymentState = PaymentState.Unpaid.ToString(),
                State = OrderState.Draft.ToString(),
                CreatedAt = DateTime.Now,
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            if(request.Items != null && request.Items.Count > 0)
            {
                var items = new List<OrderItems>();
                foreach (var item in request.Items)
                {
                    var variant = await _context.ProductVariants.FindAsync(item.VariantId);
                    if (variant == null)
                    {
                        _context.Orders.Remove(order);
                        var response = new
                        {
                            code = 404,
                            message = "Sản phẩm không tồn tại"
                        };
                        return NotFound(response);
                    }
                    var orderItem = new OrderItems
                    {
                        OrderId = order.Id,
                        VariantId = item.VariantId,
                        Amount = item.Amount,
                        StandardPrice = variant.StandardPrice,
                        SalePrice = variant.SalePrice,
                        Total = variant.SalePrice * item.Amount,
                        IsRefunded = false,
                        CreatedAt = DateTime.Now,
                    };
                    items.Add(orderItem);
                }
                await _context.OrderItems.AddRangeAsync(items);
            }
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "Tạo đơn hàng thành công",
                data = new
                {
                    order = new
                    {
                        order.Id,
                        order.CustomerId,
                        created_at = order.CreatedAt,
                        order.State,
                    }
                }
            };

            return Ok();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
