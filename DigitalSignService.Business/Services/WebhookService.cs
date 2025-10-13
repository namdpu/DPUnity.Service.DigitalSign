using DigitalSignService.Business.IServices;
using DPURedisService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DigitalSignService.Business.Services
{
    public class WebhookService : BackgroundService
    {
        private readonly RedisService _redisService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WebhookService> _logger;

        public WebhookService(
            RedisService redisService,
            IServiceProvider serviceProvider,
            ILogger<WebhookService> logger)
        {
            _redisService = redisService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WebhookService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Đăng ký lắng nghe channel Redis
                    await _redisService.SubscribeChannel("digitalsign-webhook", async (channel, message) =>
                    {
                        await HandleMessageAsync(message, stoppingToken);
                    });

                    _logger.LogInformation("Subscribed to Redis channel successfully.");

                    // Đợi cho đến khi bị hủy
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // App đang shutdown
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Redis subscription failed. Retrying in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("WebhookService stopped.");
        }

        private async Task HandleMessageAsync(string message, CancellationToken ct)
        {
            try
            {
                var jsonObject = JObject.Parse(message);
                string provider = jsonObject["provider"]?.ToString() ?? "";
                string dataString = jsonObject["data"]?.ToString() ?? "";

                _logger.LogInformation("Received message from : {provider} with data {dataString}", provider, dataString);

                using var scope = _serviceProvider.CreateScope();
                var templateService = scope.ServiceProvider.GetRequiredService<ITemplateService>();
                if (templateService is null)
                {
                    _logger.LogWarning("Not found template service");
                    return;
                }
                await templateService.HandleWebhook(provider, dataString);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON message: {Message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling message: {Message}", message);
            }
        }
    }
}
