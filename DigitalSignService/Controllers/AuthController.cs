using DigitalSignService.Business.IServices;
using DigitalSignService.DAL.DTOs.Requests.Sign;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSignService.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IJWTContext _jWTContext;
        public AuthController(IHttpContextAccessor httpContextAccessor, ILogger<DigitalSignController> logger, IJWTContext jWTContext) : base(httpContextAccessor, logger)
        {
            _jWTContext = jWTContext;
        }

        [HttpPost]
        public IActionResult Login(VTGenTokenReq req)
        {
            try
            {
                var data = _jWTContext.GenerateToken(req);
                return StatusCode(200, new
                {
                    status_code = "00",
                    message = "Thành công",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
