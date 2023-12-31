using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace asp.net.Controllers.Dashboard
{
    public abstract class SearchForm
    {
        [DefaultValue(10)]
        public int limit { get; set; }
        [DefaultValue(0)]
        public int offset { get; set; }
        public string? filter { get; set; }
        public string? keyword { get; set; }
        [JsonPropertyName("sort")]
        public string? sort { get; set; }
        [JsonPropertyName("order")]
        [DefaultValue("desc")]
        [RegularExpression("^(asc|desc)$")]
        public string? order { get; set; }

        [JsonPropertyName("start_date")]
        [DataType(DataType.Date)]
        public string? start_date { get; set; }

        [JsonPropertyName("end_date")]
        [DataType(DataType.Date)]
        public string? end_date { get; set; }

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