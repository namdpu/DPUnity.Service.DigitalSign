using DigitalSignService.DAL.DTOs.Requests;
using DigitalSignService.DAL.DTOs.Responses;
using DigitalSignService.DAL.Entities;

namespace DigitalSignService.Business.IServices
{
    public interface ITemplateService : IBaseService<Template>
    {
        Task<BaseResponse> Sign(SignReq req, string providerSign, CancellationToken cancellationToken = default);
        Task<BaseResponse> GetSignStatus(string transactionId, string providerSign, CancellationToken cancellationToken = default);
        Task<BaseResponse> CreateTemplate(CreateTemplateRequest request);
        Task<BaseResponse> GetAllTemplate();
        Task<BaseResponse> GetTemplateById(Guid id);
        Task<BaseResponse> UpdateTemplate(UpdateTemplateRequest request);
        Task<BaseResponse> GetFileSigned(string transactionId, string providerSign, CancellationToken cancellationToken = default);
        Task HandleWebhook(string providerSign, string dataString, CancellationToken cancellationToken = default);
        Task<BaseResponse> DeleteTemplate(Guid id);
        Task<BaseResponse> VerifySignaturesAsync(VerifySignatureRequest request, string providerSign, CancellationToken cancellationToken = default);
    }
}
