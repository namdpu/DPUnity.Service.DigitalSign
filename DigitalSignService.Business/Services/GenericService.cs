using DigitalSignService.Business.IServices;
using DigitalSignService.DAL.IRepository;

namespace DigitalSignService.Business.Services
{
    public abstract class GenericService<T> : IGenericService<T> where T : class
    {
        protected readonly IGenericRepository<T> rp;

        public GenericService(IGenericRepository<T> rp)
        {
            this.rp = rp;
        }
    }
}
