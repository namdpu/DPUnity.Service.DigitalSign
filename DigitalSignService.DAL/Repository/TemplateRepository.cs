using DigitalSignService.DAL.Entities;
using DigitalSignService.DAL.IRepository;
using DigitalSignService.DAL.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalSignService.DAL.Repository
{
    public class TemplateRepository : BaseRepository<Template>, ITemplateRepository
    {
        public TemplateRepository(
            IUserContext userContext,
            DataBaseContext context,
            IOptions<AppSetting> options,
            ILogger<TemplateRepository> logger
            ) : base(userContext, context, options, logger)
        {

        }
    }
}
