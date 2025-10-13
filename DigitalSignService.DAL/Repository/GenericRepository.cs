using DigitalSignService.DAL.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DigitalSignService.DAL.Repository
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DataBaseContext _context;
        protected DbSet<T> table { get; set; }

        public GenericRepository(DataBaseContext context)
        {
            _context = context;
            table = _context.Set<T>();
        }

        public virtual async Task Delete(object id)
        {
            T? existing = await table.FindAsync(id);
            if (existing != null)
            {
                table.Remove(existing);
            }
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await table.ToListAsync();
        }

        public virtual async Task<T?> GetById(object id)
        {
            return await table.FindAsync(id);
        }

        public virtual async Task Insert(T obj)
        {
            await table.AddAsync(obj);
        }

        public virtual async Task InsertRange(IEnumerable<T> objs)
        {
            await table.AddRangeAsync(objs);
        }

        public virtual async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public virtual async Task Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        public virtual async Task RemoveRange(IEnumerable<T> ids)
        {
            table.RemoveRange(ids);
        }

        public virtual async Task Reload(T obj)
        {
            await _context.Entry(obj).ReloadAsync();
        }

        public virtual IQueryable<T> Queryable()
        {
            return table.AsQueryable<T>();
        }

        public async Task<T?> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? includeConfiguration = null)
        {
            IQueryable<T> query = table;

            if (includeConfiguration != null)
            {
                query = includeConfiguration(query);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>?> GetMulWithIncludesAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? includeConfiguration = null)
        {
            IQueryable<T> query = table;

            if (includeConfiguration != null)
            {
                query = includeConfiguration(query);
            }

            return await query.Where(predicate).ToListAsync();
        }
    }
}
