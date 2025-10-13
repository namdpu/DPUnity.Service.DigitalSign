using DigitalSignService.DAL.DTOs.Requests.Sign;
using DigitalSignService.DAL.DTOs.Responses.SignDTOs;
using DigitalSignService.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalSignService.Business.Service3th
{
    public class ViettelSignApi : HttpService
    {
        private readonly ConfigAPI _configAPI;
        public ViettelSignApi(IHttpContextAccessor httpContextAccessor, ILogger<ViettelSignApi> logger, IOptions<ConfigAPI> config)
            : base(httpContextAccessor, logger, config.Value.ViettelAPI.Endpoint)
        {
            _configAPI = config.Value;
        }

        public async Task<VTLoginRes?> Login(VTLoginReq request)
            => await PostAsync<VTLoginRes>(_configAPI.ViettelAPI.Login, request);

        public async Task<VTCredentialsListRes?> GetCredentials(VTCredentialsReq request, string accessToken)
        {
            var res = await PostAsync<VTCredentialsInfoRes[]>(_configAPI.ViettelAPI.GetCredentials, request, accessToken);
            if (res == null) return null;

            return new VTCredentialsListRes
            {
                CredentialInfos = res
            };
        }

        public async Task<VTSignHashAsyncRes?> SignHash(VTSignHashReq request, string accessToken)
            => await PostAsync<VTSignHashAsyncRes>(_configAPI.ViettelAPI.SignHash, request, accessToken);

        public async Task<VTSignHashRes?> GetSignStatus(string transactionId, string accessToken)
            => await PostAsync<VTSignHashRes>(_configAPI.ViettelAPI.GetSignStatus, new { transactionId }, accessToken);

    }
}
