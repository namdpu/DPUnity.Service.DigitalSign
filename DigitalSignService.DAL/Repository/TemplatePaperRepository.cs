using DigitalSignService.DAL.Entities;
using DigitalSignService.DAL.IRepository;
using DigitalSignService.DAL.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalSignService.DAL.Repository
{
    public class TemplatePaperRepository : BaseRepository<TemplatePaper>, ITemplatePaperRepository
    {
        public TemplatePaperRepository(IUserContext userContext, DataBaseContext context, IOptions<AppSetting> options, ILogger<TemplatePaperRepository> logger) : base(userContext, context, options, logger)
        {
        }
    }
}
