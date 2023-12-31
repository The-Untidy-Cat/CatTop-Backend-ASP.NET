﻿using asp.net.Data;
using asp.net.Models;
using asp.net.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Configuration;
using System.Text.Json.Serialization;

namespace asp.net.Controllers.CustomerController
{
    public class NewOrderForm
    {
        [JsonPropertyName("address_id")]
        public int? AddressId { get; set; }

        [Required]
        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; }

        public string? Note { get; set; }

        public List<NewOrderItem>? Items { get; set; }
    }

    public class NewOrderItem
    {
        [Required]
        [JsonPropertyName("variant_id")]
        public long VariantId { get; set; }

        [Required]
        public int Amount { get; set; }
    }

    public class RatingForm
    {
        [Required]
        [IntegerValidator(ExcludeRange = false, MinValue = 1, MaxValue = 5)]
        public int rating { get; set; }
        public string? review { get; set; }
    }

    [Route("v1/customer/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DbCtx _context;
        private readonly IMailService _mailService;

        public OrderController(DbCtx context, IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var user = HttpContext.Items["user"];
            Console.WriteLine(user);
            var customer = await _context.Customers
                .Where(c => c.User.Username == user)
                .Select(c => new { c.Id })
                .FirstOrDefaultAsync();
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customer.Id)
                .Select(o => new
                {
                    o.Id,
                    created_at = o.CreatedAt,
                    o.State,
                    items = o.OrderItems.Where(oi => oi.OrderId == o.Id).Select(oi => new
                    {
                        oi.Id,
                        oi.Amount,
                        standard_price = oi.StandardPrice,
                        sale_price = oi.SalePrice,
                        oi.Total,
                        variant = new
                        {
                            oi.ProductVariant.Id,
                            oi.ProductVariant.Name,
                            product = new
                            {
                                oi.ProductVariant.Product.Id,
                                oi.ProductVariant.Product.Name,
                                oi.ProductVariant.Product.Slug,
                                oi.ProductVariant.Product.Image,
                            }
                        }
                    }).ToList(),
                    total = o.OrderItems.Where(oi => oi.OrderId == o.Id).Sum(oi => oi.Total)
                })
                .OrderByDescending(o => o.created_at)
                .ToListAsync();
            var response = new
            {
                code = 200,
                data = new
                {
                    orders
                }
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrderDetail(int id)
        {
            var user = HttpContext.Items["user"];
            Console.WriteLine(user);
            var order = _context.Orders
                .Where(o => o.Id == id && o.Customer.User.Username == user)
                .Select(o => new
                {
                    o.Id,
                    created_at = o.CreatedAt,
                    o.State,
                    payment_method = o.PaymentMethod,
                    payment_state = o.PaymentState,
                    items = o.OrderItems.Select(oi => new
                    {
                        oi.Id,
                        oi.Amount,
                        standard_price = oi.StandardPrice,
                        sale_price = oi.SalePrice,
                        oi.Total,
                        variant = new
                        {
                            oi.ProductVariant.Id,
                            oi.ProductVariant.Name,
                            product = new
                            {
                                oi.ProductVariant.Product.Id,
                                oi.ProductVariant.Product.Name,
                                oi.ProductVariant.Product.Slug,
                                oi.ProductVariant.Product.Image,
                            }
                        },
                        oi.Rating,
                        oi.Review
                    }).ToList(),
                    histories = o.OrderHistories.Select(h => new
                    {
                        h.State,
                        created_at = h.CreatedAt
                    }).ToList(),
                    address = o.AddressBook != null ? new
                    {
                        name = o.AddressBook.Name,
                        address_line = o.AddressBook.AddressLine,
                        phone = o.AddressBook.Phone,
                        province = o.AddressBook.Province,
                        district = o.AddressBook.District,
                        ward = o.AddressBook.Ward
                    } : null,
                    total = o.OrderItems.Where(oi => oi.OrderId == o.Id).Sum(oi => oi.Total)
                }).FirstOrDefault();
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

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Order>>> CreateOrder([FromBody] NewOrderForm request)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "fail",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage))
                };
                return BadRequest(response);
            }
            var user = HttpContext.Items["user"];
            var customer = await _context.Customers.Where(c => c.User.Username == user).FirstOrDefaultAsync();
            if (request.AddressId != null)
            {
                var addressBook = _context.AddressBooks.Where(a => a.Id == request.AddressId && a.CustomerId == customer.Id).FirstOrDefault();
                if (addressBook == null)
                {
                    var response = new
                    {
                        code = 404,
                        message = "Địa chỉ không tồn tại"

                    };
                    return NotFound(response);
                }
            }
            var order = new Order
            {
                CustomerId = customer.Id,
                State = OrderState.Pending.ToString(),
                PaymentMethod = request.PaymentMethod,
                ShoppingMethod = ShoppingMethod.Online.ToString(),
                PaymentState = PaymentState.Unpaid.ToString(),
                Note = request?.Note,
                AddressId = request?.AddressId ?? null,
                CreatedAt = DateTime.Now
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            if (request.Items != null && request.Items.Count > 0)
            {
                var items = new List<OrderItems>();
                foreach (NewOrderItem item in request?.Items)
                {
                    Console.WriteLine(item.VariantId);
                    var existedItems = order.OrderItems?.Where(oi => oi.VariantId == item.VariantId).ToList();
                    if (existedItems != null)
                        continue;
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
                        CreatedAt = DateTime.Now
                    };
                    items.Add(orderItem);
                }
                await _context.OrderItems.AddRangeAsync(items);
            }
            else
            {
                var cart = await _context.Carts.Where(c => c.CustomerID == customer.Id).ToListAsync();
                if (cart.Count == 0)
                {
                    _context.Orders.Remove(order);
                    return BadRequest(new
                    {
                        code = 400,
                        message = "Giỏ hàng trống"

                    });
                }
                var items = new List<OrderItems>();
                foreach (Cart item in cart)
                {
                    var variant = await _context.ProductVariants.FindAsync(item.VariantId);
                    if (variant == null)
                    {
                        _context.Orders.Remove(order);
                        var response = new
                        {
                            code = 400,
                            message = "Sản phẩm không tồn tại"
                        };
                        return BadRequest(response);
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
                        CreatedAt = DateTime.Now
                    };
                    items.Add(orderItem);
                }
                await _context.OrderItems.AddRangeAsync(items);
                _context.Carts.RemoveRange(cart);
            }
            await _context.SaveChangesAsync();
            var history = new OrderHistories
            {
                OrderId = order.Id,
                State = OrderState.Pending.ToString(),
                CreatedAt = DateTime.Now
            };
            await _context.OrderHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates/order_item.html");
            var content = "<p>Đơn hàng của bạn đã được tạo thành công</p>";
            content += "<p>Để xem chi tiết đơn hàng, vui lòng truy cập vào <strong><a href='https://cattop.theuntidycat.tech/user/orders/" + order.Id + "'>đây</a></strong></p>";
            content += "<p>Thông tin đơn hàng:<br>- Mã đơn hàng: " +
                order.Id + "<br>- Ngày tạo: " + order.CreatedAt + "<br>- Thành tiền: " + CustomService.FormatVietnameseCurrency((double)order?.OrderItems?.Sum(oi => oi.Total));
            content += "<p>Đơn hàng của bạn có các sản phẩm sau:</p>";
            foreach (var item in order.OrderItems)
            {
                string emailTemplateText = await System.IO.File.ReadAllTextAsync(filePath);
                var product = await _context.Products.Where(p => p.Id == item.ProductVariant.ProductID).FirstOrDefaultAsync();
                emailTemplateText = emailTemplateText.Replace("{#product-image#}", item.ProductVariant.Image);
                emailTemplateText = emailTemplateText.Replace("{#product-name#}", product.Name);
                emailTemplateText = emailTemplateText.Replace("{#variant-name#}", item?.ProductVariant.Name);
                emailTemplateText = emailTemplateText.Replace("{#sale_price#}", CustomService.FormatVietnameseCurrency((double)item.SalePrice));
                emailTemplateText = emailTemplateText.Replace("{#amount#}", item.Amount.ToString());
                emailTemplateText = emailTemplateText.Replace("{#total#}", CustomService.FormatVietnameseCurrency((double)item.Total));
                content += "<br>" + emailTemplateText;
            }
            var HTMLData = new HTMLMailData
            {
                Email = customer.Email,
                Subject = "CatTop đã nhận đơn hàng #" + order.Id,
                Content = content,
            };
            _mailService.SendHTMLMailAsync(HTMLData);

            var responseSuccess = new
            {
                code = 200,
                message = "success",
                data = new
                {
                    detail = new
                    {
                        order.Id,
                        created_at = order.CreatedAt,
                        order.State,
                    }
                }
            };
            return Ok(responseSuccess);
        }

        [HttpPost("{orderId}/items/{itemId}/rate")]
        public async Task<ActionResult<IEnumerable<Order>>> RateItems(int orderId, int itemId, [FromBody] RatingForm request)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "fail",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage))
                };
                return BadRequest(response);
            }
            var user = HttpContext.Items["user"];
            var customer = await _context.Customers.Where(c => c.User.Username == user).FirstOrDefaultAsync();
            var order = _context.Orders.Where(o => o.Id == orderId && o.CustomerId == customer.Id).FirstOrDefault();
            if (order == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Đơn hàng không tồn tại"
                };
                return NotFound(response);
            }
            var orderItem = _context.OrderItems.Where(oi => oi.Id == itemId && oi.OrderId == orderId).FirstOrDefault();
            if (orderItem == null)
            {
                var response = new
                {
                    code = 404,
                    message = "Sản phẩm không tồn tại"
                };
                return NotFound(response);
            }
            if (orderItem.Rating != null)
            {
                var response = new
                {
                    code = 400,
                    message = "Sản phẩm đã được đánh giá"
                };
                return BadRequest(response);
            }
            orderItem.Rating = request.rating.ToString();
            orderItem.Review = request.review;
            await _context.SaveChangesAsync();
            var responseSuccess = new
            {
                code = 200,
                message = "success",
                data = new
                {
                    order = new
                    {
                        order.Id,
                        created_at = order.CreatedAt,
                        order.State,
                    }
                }
            };
            return Ok(responseSuccess);
        }
    }
}
