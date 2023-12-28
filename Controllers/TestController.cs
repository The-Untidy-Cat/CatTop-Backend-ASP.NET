using asp.net.Models;
using asp.net.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace asp.net.Controllers
{
    [Route("test")]
    [ApiController]

    public class TestController : ControllerBase
    {

        private readonly IMailService _mailService;
        //injecting the IMailService into the constructor
        public TestController(IMailService _MailService)
        {
            _mailService = _MailService;
        }

        [HttpPost("/send-mail")]
        public async Task<ActionResult> SendHTMLMail(HTMLMailData mailData)
        {
            string filePath = Directory.GetCurrentDirectory() + "\\Templates\\index.html";

            string emailTemplateText = await System.IO.File.ReadAllTextAsync(filePath);
            var check = await _mailService.SendHTMLMailAsync(mailData);
            if (!check)
            {
                return BadRequest(emailTemplateText);
            }
            return Ok();
        }
    }
}
