using DigitalSignService.DAL.Models;

namespace DigitalSignService.DAL.IRepository
{
    public interface IBaseRepository<T> : IGenericRepository<T> where T : class
    {
        public UserContextInfo UserInfo { get; }
        Task SoftDelete(object id);
        Task SoftDeleteRange(IEnumerable<T> objs);
        Task UpdateWithoutTracking(T obj);
        Task<T?> GetActiveById(object id);
    }
}
