using DigitalSignService.DAL.DTOs.Requests;
using DigitalSignService.DAL.DTOs.Responses;

namespace DigitalSignService.Business.IServices.Sign
{
    public interface ISigningProvider
    {
        string Name { get; }  // "viettel", "vnpt", "fpt"
        Task<string> SignCAPDF(SignReq req, CancellationToken cancellationToken = default);
        Task<SignStatusDTO> GetSignStatus(SignReq req, string transactionId, CancellationToken cancellationToken = default);
        Task<SignStatusDTO> HandleWebhook(string dataString, string documentName, CancellationToken cancellationToken);
    }
}
