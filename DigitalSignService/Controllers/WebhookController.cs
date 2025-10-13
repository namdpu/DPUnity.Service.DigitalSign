using DPURedisService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalSignService.Controllers
{
    [Authorize(AuthenticationSchemes = "DigitalSignAuth")]
    public class WebhookController : BaseController
    {
        private readonly RedisService _redisService;
        public WebhookController(IHttpContextAccessor httpContextAccessor, ILogger<WebhookController> logger, RedisService redisService) : base(httpContextAccessor, logger)
        {
            _redisService = redisService;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveData(object data)
        {
            try
            {
                string provider = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "provider")?.Value ?? string.Empty;
                if (String.IsNullOrEmpty(provider))
                {
                    return BadRequest("Provider not found");
                }
                string stringData = JsonConvert.SerializeObject(new
                {
                    provider = provider,
                    data = data
                });
                Console.WriteLine(stringData);
                _logger.LogInformation("ReceiveTranslate: " + stringData);
                await _redisService.PublishMessage(stringData, "digitalsign-webhook");

                return StatusCode(200, new
                {
                    status_code = "00",
                    message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                return StatusCode(500, "Lỗi hệ thống");
            }
        }
    }
}
