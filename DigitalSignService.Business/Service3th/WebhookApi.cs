using DigitalSignService.DAL.DTOs.Requests;
using DigitalSignService.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalSignService.Business.Service3th
{
    public class WebhookApi : HttpService
    {
        private readonly ConfigAPI _configAPI;
        private readonly AppSetting _appSetting;
        public WebhookApi(IHttpContextAccessor httpContextAccessor, ILogger<WebhookApi> logger, IOptions<ConfigAPI> config, IOptions<AppSetting> appSetting) : base(httpContextAccessor, logger, config.Value.WebHookAPI.Endpoint)
        {
            _configAPI = config.Value;
            _appSetting = appSetting.Value;
        }

        public async Task<string> PushMessage(PushMessReq request)
        {
            var res = await PostAsync<HttpResponseMessage>($"Publisher/{_appSetting.PublisherId}/{_configAPI.WebHookAPI.PushMessage}", request,
                customHeaders: new Dictionary<string, string> {
                    {"api_key", _appSetting.WebHookApiKey } });

            if (res == null) return "Response message is null";

            if (res.IsSuccessStatusCode)
                return string.Empty;

            return await res.Content.ReadAsStringAsync();
        }
    }
}
