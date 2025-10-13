using System.Linq.Expressions;

namespace DigitalSignService.DAL.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(object id);
        Task Insert(T obj);
        Task InsertRange(IEnumerable<T> objs);
        Task Update(T obj);
        Task Delete(object id);
        Task Save();
        Task RemoveRange(IEnumerable<T> objs);
        Task Reload(T obj);
        IQueryable<T> Queryable();
        Task<T?> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? includeConfiguration = null);
        Task<List<T>?> GetMulWithIncludesAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? includeConfiguration = null);
    }
}
