using DigitalSignService.DAL.DTOs.Requests;
using DigitalSignService.DAL.DTOs.Responses;
using DigitalSignService.DAL.Entities;

namespace DigitalSignService.Business.IServices
{
    public interface IPaperSizeService : IBaseService<PaperSize>
    {
        Task<BaseResponse> GetAllPaperSize();
        Task<BaseResponse> CustomPage(CustomPageRequest request);
        Task<BaseResponse> DeletePaperSize(Guid id);
    }
}
