using DigitalSignService.DAL.Entities;
using DigitalSignService.DAL.IRepository;
using DigitalSignService.DAL.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalSignService.DAL.Repository
{
    public class HistorySignRepository : BaseRepository<HistorySign>, IHistorySignRepository
    {
        public HistorySignRepository(
            IUserContext userContext,
            DataBaseContext context,
            IOptions<AppSetting> options,
            ILogger<HistorySignRepository> logger
            ) : base(userContext, context, options, logger)
        {

        }
    }
}
