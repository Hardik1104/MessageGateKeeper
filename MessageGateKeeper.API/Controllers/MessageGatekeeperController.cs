using MessageGateKeeper.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessageGateKeeper.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageGatekeeperController : Controller
    {
        private readonly RateLimiterService _rateLimiterService;

        public MessageGatekeeperController(RateLimiterService rateLimiterService)
        {
            _rateLimiterService = rateLimiterService;
        }

        [HttpGet("can-send")]
        public IActionResult CanSend([FromQuery] string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return BadRequest("Phone number is required.");

            var canSend = _rateLimiterService.CanSendMessage(phoneNumber);
            return Ok(new { CanSend = canSend });
        }
    }
}
