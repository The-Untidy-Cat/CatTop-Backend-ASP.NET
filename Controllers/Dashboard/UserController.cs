using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asp.net.Data;
using asp.net.Models;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace asp.net.Controllers.CustomerController
{

    [Route("v1/dashboard/user")]
    [ApiController]
    public class DashUserController : ControllerBase
    {
        private readonly DbCtx _context;

        public DashUserController(DbCtx context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetProfile()
        {
            var user = HttpContext.Items["user"];
            var employee = await _context.Employees.Where(c => c.User.Username == user).Select(c => new
            {
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.PhoneNumber,
                c.DateOfBirth,
                c.Gender,
                c.User.Username
            }).FirstOrDefaultAsync();
            var response = new
            {
                code = 200,
                message = "Success",
                data = new
                {
                    user = new
                    {
                        id = employee?.Id,
                        first_name = employee?.FirstName,
                        last_name = employee?.LastName,
                        email = employee?.Email,
                        phone_number = employee?.PhoneNumber,
                        employee?.Gender,
                        date_of_birth = employee?.DateOfBirth,
                        username = employee?.Username
                    }

                }
            };
            return Ok(response);
        }
    }
}
