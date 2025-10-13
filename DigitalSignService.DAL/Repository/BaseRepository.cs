using DigitalSignService.DAL.Entities;
using DigitalSignService.DAL.IRepository;
using DigitalSignService.DAL.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalSignService.DAL.Repository
{
    public abstract class BaseRepository<T> : GenericRepository<T>, IBaseRepository<T> where T : class
    {
        protected readonly AppSetting _appSetting;
        protected readonly ILogger<BaseRepository<T>> logger;
        protected readonly IUserContext _userContext;
        public BaseRepository(IUserContext userContext, DataBaseContext context, IOptions<AppSetting> options, ILogger<BaseRepository<T>> logger) : base(context)
        {
            _userContext = userContext;
            _appSetting = options.Value;
            this.logger = logger;
        }

        public virtual async Task SoftDelete(object id)
        {
            if (typeof(AuditEntity).IsAssignableFrom(typeof(T)))
            {
                T? existing = await table.FindAsync(id);
                if (existing != null)
                {
                    if (existing is AuditEntity entity)
                    {
                        entity.IsDeleted = true;
                        entity.UpdatedDateTime = DateTime.UtcNow;
                        entity.UpdatedUserId = UserInfo.Id;
                        await this.Update((T)(object)entity);
                    }
                }
            }
        }

        public virtual async Task SoftDeleteRange(IEnumerable<T> objs)
        {
            if (typeof(AuditEntity).IsAssignableFrom(typeof(T)))
            {
                foreach (var obj in objs)
                {
                    if (obj != null)
                    {
                        if (obj is AuditEntity entity)
                        {
                            entity.IsDeleted = true;
                            entity.UpdatedDateTime = DateTime.UtcNow;
                            entity.UpdatedUserId = UserInfo.Id;
                            await this.Update((T)(object)entity);
                        }
                    }
                }
            }
        }

        public override async Task Insert(T obj)
        {
            if (typeof(AuditEntity).IsAssignableFrom(typeof(T)))
            {
                if (obj is AuditEntity entity)
                {
                    entity.CreatedDateTime = DateTime.UtcNow;
                    entity.CreatedUserId = UserInfo.Id;
                    await base.Insert(obj);
                }
            }
            else
            {
                await base.Insert(obj);
            }
        }

        public override async Task Update(T obj)
        {
            if (typeof(AuditEntity).IsAssignableFrom(typeof(T)))
            {
                if (obj is AuditEntity entity)
                {
                    entity.UpdatedDateTime = DateTime.UtcNow;
                    entity.UpdatedUserId = UserInfo.Id;
                    await base.Update(obj);
                }
            }
            else
            {
                await base.Update(obj);
            }
        }

        public async Task UpdateWithoutTracking(T obj)
        {
            if (typeof(AuditEntity).IsAssignableFrom(typeof(T)))
            {
                if (obj is AuditEntity entity)
                {
                    entity.UpdatedDateTime = DateTime.UtcNow;
                    await base.Update(obj);
                }
            }
        }

        public virtual async Task<T?> GetActiveById(object id)
        {
            if (typeof(AuditEntity).IsAssignableFrom(typeof(T)))
            {
                T? existing = await table.FindAsync(id);
                if (existing != null)
                {
                    if (existing is AuditEntity entity)
                    {
                        return !entity.IsDeleted && entity.IsActive ? existing : null;
                    }

                    return null;
                }
            }

            return null;
        }

        public UserContextInfo UserInfo
        {
            get
            {
                return _userContext.UserInfo;
            }
        }
    }
}
