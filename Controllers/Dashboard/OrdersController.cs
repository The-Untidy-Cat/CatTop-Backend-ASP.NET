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
    public class UpdateOrderForm
    {
        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonPropertyName("tracking_no")]
        public string TrackingNo { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("payment_state")]
        public string PaymentState { get; set; }
    }
    public class NewOrderItemForm
    {
        [Required]
        [JsonPropertyName("variant_id")]
        public int VariantId { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
    }

    public class UpdateOrderItemForm
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
        
        [JsonPropertyName("serial_number")]
        public string SerialNumber { get; set; }
    }
    public class NewDashOrderForm
    {
        [Required]
        [JsonPropertyName("customer_id")]
        public int CustomerId { get; set; }

        [Required]
        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; }

        public List<NewDashOrderItem>? Items { get; set; }
    }

    public class NewDashOrderItem
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
                    } ?? null,
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
                data = order
            };    

            return Ok(response);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderForm request)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "Faile in UpdateOrder Dash",
                    data = new
                    {
                        errors = ModelState.Values.SelectMany(t => t.Errors.Select(e => e.ErrorMessage))
                    }
                };
                return BadRequest(response);
            }

            var order = await _context.Orders.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy đơn hàng"
                };
                return NotFound(response);
            }
            if(request.PaymentMethod != null)
            {
                order.PaymentMethod = request.PaymentMethod;
            }
            if(request.TrackingNo != null)
            {
                order.TrackingNo = request.TrackingNo;
            }
            if(request.Note != null)
            {
                order.Note = request.Note;
            }
            if(request.State != null)
            {
                order.State = request.State;
            }
            if(request.PaymentState != null)
            {
                order.PaymentState = request.PaymentState;
            }

            order.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "Cập nhật đơn hàng thành công",
                data = new
                {
                    order = new
                    {
                        order.Id,
                        order.ShoppingMethod,
                        order.PaymentMethod,
                        order.PaymentState,
                        order.State,
                        order.TrackingNo,
                        order.AddressId,
                        order.Note,
                        order.CreatedAt,
                        order.UpdatedAt,
                    }
                }
            };
            return Ok(responseSuccess);
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Order>>> CreateOrder([FromBody] NewDashOrderForm request)
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
            var employee = await _context.Employees.Where(e => e.User.Username == user).FirstOrDefaultAsync();
            var customer = await _context.Customers.Where(c => c.Id == request.CustomerId).FirstOrDefaultAsync();
            var order = new Order
            {
                CustomerId = customer.Id,
                EmployeeId = employee?.Id ?? null,
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
                    payment_method = order.PaymentMethod,
                    state = order.State,
                    payment_state = order.PaymentState,
                    shopping_method = order.ShoppingMethod,
                    updated_at = order.UpdatedAt,
                    created_at = order.CreatedAt,
                    id = order.Id,
                    total = order.OrderItems.Where(i => i.OrderId == i.Id).Sum(i => i.Total),
                }
            };

            return Ok(responseSuccess);
        }

        [HttpPost("{id}/items")]
        public async Task<ActionResult<IEnumerable<OrderItems>>> CreateOrderItems([FromBody] NewOrderItemForm request, int id)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "Fail in CreateOrderItems",
                    errors = ModelState.Values.SelectMany(t => t.Errors.Select(e => e.ErrorMessage))
                };
                return BadRequest(response);
            }

            var variant = await _context.ProductVariants.Where(ProductVariants => ProductVariants.Id == request.VariantId).FirstOrDefaultAsync();
            var order = await _context.Orders.Where(Orders => Orders.Id == id).FirstOrDefaultAsync();

            if(order == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy đơn hàng"
                };
                return BadRequest(response);
            }

            if(order.State == OrderState.Confirmed.ToString())
            {
                var response = new
                {
                    code = 400,
                    message = "Đơn hàng đã được xác nhận, không thể thêm sản phẩm"
                };
                return BadRequest(response);
            }

            var orderItems = new OrderItems
            {
                OrderId = id,
                VariantId = variant.Id,
                Amount = request.Amount,
                StandardPrice = variant.StandardPrice,
                Total = variant.StandardPrice * request.Amount,
                IsRefunded = false,
                Rating = null,
                SerialNumber = null,
                Review = null,
                CreatedAt = DateTime.Now,
            };
            await _context.OrderItems.AddAsync(orderItems);
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "Success in CreateOrderItems",
                data = new
                {
                    orderItems = new
                    {
                        orderItems.Id,
                        order_id = order.Id,
                        orderItems.VariantId,
                        orderItems.Amount,
                        orderItems.StandardPrice,
                        orderItems.SalePrice,
                        orderItems.Total,
                        orderItems.IsRefunded,
                        orderItems.Rating,
                        orderItems.Review,
                        orderItems.SerialNumber,
                        orderItems.CreatedAt,
                        orderItems.UpdatedAt,
                    }
                }
            };

            return Ok(responseSuccess);
        }

        [HttpPut("{id}/items/{itemId}")]
        public async Task<IActionResult> UpdateOrderItem(int id, int itemId, [FromBody] UpdateOrderItemForm request)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "Fail in UpdateOrderItem",
                    errors = ModelState.Values.SelectMany(t => t.Errors.Select(e => e.ErrorMessage))
                };
                return BadRequest(response);
            }

            var order = await _context.Orders.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy đơn hàng"
                };
                return NotFound(response);
            }

            var orderItem = await _context.OrderItems.Where(i => i.Id == itemId).FirstOrDefaultAsync();
            if (orderItem == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy sản phẩm"
                };
                return NotFound(response);
            }

            if (order.State == OrderState.Confirmed.ToString())
            {
                var response = new
                {
                    code = 400,
                    message = "Đơn hàng đã được xác nhận, không thể thay đổi sản phẩm"
                };
                return BadRequest(response);
            }

            if (request.Amount != 0)
            {
                orderItem.Amount = request.Amount;
                orderItem.Total = orderItem.SalePrice * request.Amount;
            }
            if (request.SerialNumber != null)
            {
                orderItem.SerialNumber = request.SerialNumber;
            }
            orderItem.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "Cập nhật sản phẩm thành công",
                data = new
                {
                    orderItem = new
                    {
                        orderItem.Id,
                        orderItem.OrderId,
                        orderItem.VariantId,
                        orderItem.Amount,
                        orderItem.StandardPrice,
                        orderItem.SalePrice,
                        orderItem.Total,
                        orderItem.IsRefunded,
                        orderItem.Rating,
                        orderItem.Review,
                        orderItem.SerialNumber,
                        orderItem.CreatedAt,
                        orderItem.UpdatedAt,
                    }
                }
            };
            return Ok(responseSuccess);
        }

        [HttpDelete("{id}/items/{itemId}")]
        public async Task<IActionResult> DeleteOrderItem(int id, int itemId)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "Fail in DeleteOrderItem",
                    errors = ModelState.Values.SelectMany(t => t.Errors.Select(e => e.ErrorMessage))
                };
                return BadRequest(response);
            }

            var order = await _context.Orders.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy đơn hàng"
                };
                return NotFound(response);
            }

            var orderItem = await _context.OrderItems.Where(i => i.Id == itemId).FirstOrDefaultAsync();
            if (orderItem == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Không tìm thấy sản phẩm"
                };
                return NotFound(response);
            }

            if (order.State == OrderState.Confirmed.ToString())
            {
                var response = new
                {
                    code = 400,
                    message = "Đơn hàng đã được xác nhận, không thể xóa sản phẩm"
                };
                return BadRequest(response);
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "Xóa sản phẩm thành công",
                data = new
                {
                    orderItem = new
                    {
                        orderItem.Id,
                        orderItem.OrderId,
                        orderItem.VariantId,
                        orderItem.Amount,
                        orderItem.StandardPrice,
                        orderItem.SalePrice,
                        orderItem.Total,
                        orderItem.IsRefunded,
                        orderItem.Rating,
                        orderItem.Review,
                        orderItem.SerialNumber,
                        orderItem.CreatedAt,
                        orderItem.UpdatedAt,
                    }
                }
            };
            return Ok(responseSuccess);
        }
        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
