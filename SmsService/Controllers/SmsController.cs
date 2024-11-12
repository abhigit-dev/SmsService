using Microsoft.AspNetCore.Mvc;
using SmsService.Services;
using SmsService.Models;

namespace SmsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly Services.SmsService _smsService;

        public SmsController(Services.SmsService smsService)
        {
            _smsService = smsService;
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> SendSms([FromBody] SendSmsCommand command)
        {
            var result = await _smsService.SendSmsAsync(command);
            if (result)
            {
                return Ok("SMS sent successfully");
            }
            else
            {
                return StatusCode(500, "Failed to send SMS");
            }
        }
    }
}