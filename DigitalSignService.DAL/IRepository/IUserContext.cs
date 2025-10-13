using DigitalSignService.DAL.Models;

namespace DigitalSignService.DAL.IRepository
{
    public interface IUserContext
    {
        UserContextInfo UserInfo { get; }
    }
}
