using asp.net.Data;
using asp.net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace asp.net.Controllers.CustomerController
{
    public class NewCartForm
    {
        [Required]
        [JsonPropertyName("variant_id")]
        public long VariantID { get; set; }
        public int Amount { get; set; }
    }

    [Route("v1/customer/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly DbCtx _context;
        public CartController(DbCtx context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCart()
        {
            var user = HttpContext.Items["user"];
            var customer = await _context.Customers.Where(c => c.User.Username == user).FirstOrDefaultAsync();
            var cart = await _context.Carts.Where(c => c.CustomerID == customer.Id)
                .Select(c => new
                {
                    variant = new
                    {
                        c.Variant.Id,
                        c.Variant.Name,
                        sale_price = c.Variant.SalePrice,
                        c.Variant.Discount,
                        standard_price = c.Variant.StandardPrice,
                        c.Variant.Image,
                        c.Variant.SKU,
                        c.Variant.State,
                        product = new
                        {
                            c.Variant.Product.Id,
                            c.Variant.Product.Name,
                            c.Variant.Product.Slug,
                            c.Variant.Product.Image,
                            c.Variant.Product.State
                        }
                    }
                })
                .ToListAsync();
            var response = new
            {
                code = 200,
                message = "success",
                data = new
                {
                    cart
                }
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Cart>>> CreateCart([FromBody] NewCartForm request)
        {
            if (!ModelState.IsValid)
            {
                var response = new
                {
                    code = 400,
                    message = "fail",
                    data = new
                    {
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage))
                    }
                };
                return BadRequest(response);
            }
            var user = HttpContext.Items["user"];
            var customer = await _context.Customers.Where(c => c.User.Username == user)
                .Select(c => new { c.Id })
                .FirstOrDefaultAsync();
            var variant = await _context.ProductVariants.FindAsync(request.VariantID);
            if (variant == null)
            {
                var response = new
                {
                    code = 404,
                    message = "fail",
                    errors = "Variant not found"
                };
                return NotFound(response);
            }
            if (request.Amount < 1)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = "fail",
                    errors = "Amount must be greater than 0"
                });
            }
            var cart = await _context.Carts.Where(c => c.CustomerID == customer.Id && c.VariantId == variant.Id).FirstOrDefaultAsync();
            if (cart != null)
            {
                cart.Amount += request.Amount;
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
                var newCart = await _context.Carts.Where(c => c.CustomerID == customer.Id).Select(c => new
                {
                    c.Id,
                    c.Amount,
                    variant = new
                    {
                        c.Variant.Id,
                        c.Variant.Name,
                        sale_price = c.Variant.SalePrice,
                        c.Variant.Discount,
                        standard_price = c.Variant.StandardPrice,
                        c.Variant.Image,
                        c.Variant.SKU,
                        c.Variant.State,
                        product = new
                        {
                            c.Variant.Product.Id,
                            c.Variant.Product.Name,
                            c.Variant.Product.Slug,
                            c.Variant.Product.Image,
                            c.Variant.Product.State
                        }
                    }
                })
                    .ToListAsync();
                var response = new
                {
                    code = 200,
                    message = "success",
                    data = new
                    {
                        cart = newCart
                    }
                };
                return Ok(response);
            }
            cart = new Cart
            {
                CustomerID = customer.Id,
                VariantId = variant.Id,
                Amount = request.Amount
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            var newCart1 = await _context.Carts.Where(c => c.CustomerID == customer.Id).Select(c => new
            {
                c.Id,
                c.Amount,
                variant = new
                {
                    c.Variant.Id,
                    c.Variant.Name,
                    sale_price = c.Variant.SalePrice,
                    c.Variant.Discount,
                    standard_price = c.Variant.StandardPrice,
                    c.Variant.Image,
                    c.Variant.SKU,
                    c.Variant.State,
                    product = new
                    {
                        c.Variant.Product.Id,
                        c.Variant.Product.Name,
                        c.Variant.Product.Slug,
                        c.Variant.Product.Image,
                        c.Variant.Product.State
                    }
                }
            })
            .ToListAsync();
            var response1 = new
            {
                code = 200,
                message = "success",
                data = new
                {
                    cart = newCart1
                }
            };
            return Ok(response1);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<Cart>>> UpdateCart(int id, int amount)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                var response = new
                {
                    code = 404,
                    message = "fail",
                    errors = "Cart not found"
                };
                return NotFound(response);
            }
            if (amount < 0)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = "fail",
                    errors = "Amount must be greater than 0"
                });
            }
            if (amount == 0)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                var newCart = await _context.Carts.Where(c => c.Id == id).Select(c => new
                {
                    c.Id,
                    c.Amount,
                    variant = new
                    {
                        c.Variant.Id,
                        c.Variant.Name,
                        sale_price = c.Variant.SalePrice,
                        c.Variant.Discount,
                        standard_price = c.Variant.StandardPrice,
                        c.Variant.Image,
                        c.Variant.SKU,
                        c.Variant.State,
                        product = new
                        {
                            c.Variant.Product.Id,
                            c.Variant.Product.Name,
                            c.Variant.Product.Slug,
                            c.Variant.Product.Image,
                            c.Variant.Product.State
                        }
                    }
                }).ToListAsync();
                var response = new
                {
                    code = 200,
                    message = "success",
                    data = new
                    {
                        cart = newCart
                    }
                };
                return Ok(response);
            }
            cart.Amount = amount;
            await _context.SaveChangesAsync();
            var newCart1 = await _context.Carts.Where(c => c.Id == id).Select(c => new
            {
                c.Id,
                c.Amount,
                variant = new
                {
                    c.Variant.Id,
                    c.Variant.Name,
                    sale_price = c.Variant.SalePrice,
                    c.Variant.Discount,
                    standard_price = c.Variant.StandardPrice,
                    c.Variant.Image,
                    c.Variant.SKU,
                    c.Variant.State,
                    product = new
                    {
                        c.Variant.Product.Id,
                        c.Variant.Product.Name,
                        c.Variant.Product.Slug,
                        c.Variant.Product.Image,
                        c.Variant.Product.State
                    }
                }
            }).ToListAsync();
            var response1 = new
            {
                code = 200,
                message = "success",
                data = new
                {
                    cart = newCart1
                }
            };
            return Ok(response1);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<Cart>>> DeleteCart()
        {
            var user = HttpContext.Items["user"];
            var customer = await _context.Customers.Where(c => c.User.Username == user)
                .Select(c => new { c.Id })
                .FirstOrDefaultAsync();
            var cart = await _context.Carts.Where(c => c.CustomerID == customer.Id).ToListAsync();
            if (cart == null)
            {
                var response = new
                {
                    code = 200,
                    message = "success",
                    data = new
                    {
                        cart = new List<Array>()
                    }
                };
                return Ok(response);
            }
            _context.Carts.RemoveRange(cart);
            await _context.SaveChangesAsync();
            var response1 = new
            {
                code = 200,
                message = "success",
                data = new
                {
                    cart = new List<Array>()
                }
            };
            return Ok(response1);
        }
    }
}
