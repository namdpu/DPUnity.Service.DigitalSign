using DigitalSignService.DAL.IRepository;
using DigitalSignService.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DigitalSignService.DAL.Repository
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserContext> _logger;
        private UserContextInfo _userInfo;

        public UserContext(IHttpContextAccessor httpContextAccessor, ILogger<UserContext> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public UserContextInfo UserInfo
        {
            get
            {
                return _userInfo ??= BuildUserContextInfo();
            }
        }

        private UserContextInfo BuildUserContextInfo()
        {
            try
            {
                var listClaims = _httpContextAccessor.HttpContext.User.Claims.ToList();
                var user = new UserContextInfo
                {
                    Id = new Guid(listClaims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value),
                    AccessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", ""),
                };
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid token, can not get user info");
            }
        }
    }
}
