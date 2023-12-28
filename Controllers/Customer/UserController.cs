using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;

namespace asp.net.Controllers.CustomerController
{
    [Route("v1/customers")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DbCtx _context;

        public UserController(DbCtx context)
        {
            _context = context;
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var user = HttpContext.Items["user"];
            return Ok(user);
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
