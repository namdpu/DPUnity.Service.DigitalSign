using DigitalSignService.DAL.Entities;
using DigitalSignService.DAL.IRepository;
using DigitalSignService.DAL.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalSignService.DAL.Repository
{
    public class TemplatePaperUserSignRepository : BaseRepository<TemplatePaperUserSign>, ITemplatePaperUserSignRepository
    {
        public TemplatePaperUserSignRepository(IUserContext userContext, DataBaseContext context, IOptions<AppSetting> options, ILogger<TemplatePaperUserSignRepository> logger)
            : base(userContext, context, options, logger)
        {
        }
    }
}
