using DigitalSignService.DAL.Models;
using DPUStorageService.APIs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalSignService.Business.Services.Sign
{
    public class VnptSigningProvider : BaseSigningProvider
    {
        public override string Name => "vnpt";
        public VnptSigningProvider(ILogger<VnptSigningProvider> _logger, IOptions<DigitalSignSettings> settings, CachingService cachingService, IApiStorage apiStorage, IOptions<AppSetting> options1) : base(_logger, settings, cachingService, apiStorage, options1)
        {
        }
    }
}