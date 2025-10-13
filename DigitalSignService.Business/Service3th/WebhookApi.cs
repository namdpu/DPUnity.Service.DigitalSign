using DigitalSignService.DAL.DTOs.Requests;
using DigitalSignService.DAL.DTOs.Requests.Sign;
using DigitalSignService.DAL.DTOs.Responses.SignDTOs;
using DigitalSignService.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignService.Business.Service3th
{
    public class WebhookApi : HttpService
    {
        private readonly ConfigAPI _configAPI;
        private readonly AppSetting _appSetting;
        public WebhookApi(IHttpContextAccessor httpContextAccessor, ILogger logger, string baseEndpoint, IOptions<ConfigAPI> config, IOptions<AppSetting> appSetting) : base(httpContextAccessor, logger, baseEndpoint)
        {
            _configAPI = config.Value;
            _appSetting = appSetting.Value;
        }

        public async Task<string> PushMessage(PushMessReq request)
        {
            var res = await PostAsync<HttpResponseMessage>(_configAPI.WebHookAPI.PushMessage, request,
                customHeaders: new Dictionary<string, string> {
                    {"api_key", _appSetting.WebHookApiKey } });

            if (res == null) return "Response message is null";

            if(res.IsSuccessStatusCode)
                return string.Empty;

            return await res.Content.ReadAsStringAsync();
        }
    }
}
