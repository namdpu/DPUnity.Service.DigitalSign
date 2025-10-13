using DigitalSignService.DAL.DTOs.Requests.Sign;
using DigitalSignService.DAL.DTOs.Responses.SignDTOs;

namespace DigitalSignService.Business.IServices
{
    public interface IJWTContext
    {
        VTGenTokenRes GenerateToken(VTGenTokenReq request);
    }
}
