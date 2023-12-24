using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;

namespace asp.net.Controllers.Dashboard
{
    public class SearchForm
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public string? filter { get; set; }
        public string? keyword { get; set; }
    }
    [Route("v1/dashboard/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly DbCtx _context;

        public CustomersController(DbCtx context)
        {
            _context = context;
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}