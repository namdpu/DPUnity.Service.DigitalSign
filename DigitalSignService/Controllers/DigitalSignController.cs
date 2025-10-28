using DigitalSignService.Business.IServices;
using DigitalSignService.DAL.DTOs.Requests;
using DigitalSignService.DAL.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DigitalSignService.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthGateway")]
    public class DigitalSignController : BaseController
    {
        private readonly ITemplateService _templateService;
        private readonly IPaperSizeService _pageSizeService;
        public DigitalSignController(IHttpContextAccessor httpContextAccessor, ITemplateService templateService, ILogger<DigitalSignController> logger, IPaperSizeService paperSizeService) : base(httpContextAccessor, logger)
        {
            _templateService = templateService;
            _pageSizeService = paperSizeService;
        }

        [SwaggerOperation("Lấy danh sách pagesize")]
        [HttpGet]
        public async Task<BaseResponse> GetAllPaperSize()
            => await _pageSizeService.GetAllPaperSize();

        [SwaggerOperation("Tạo một custome pagesize")]
        [HttpPost]
        public async Task<BaseResponse> CustomPageSize(CustomPageRequest request)
            => await _pageSizeService.CustomPage(request);

        [SwaggerOperation("Xóa pagesize")]
        [HttpDelete]
        public async Task<BaseResponse> DeletePaperSize(Guid id)
            => await _pageSizeService.DeletePaperSize(id);

        [SwaggerOperation("Tạo template")]
        [HttpPost]
        public async Task<BaseResponse> CreateTemplate(CreateTemplateRequest request)
            => await _templateService.CreateTemplate(request);

        [SwaggerOperation("Get All template")]
        [HttpGet]
        public async Task<BaseResponse> GetAllTemplate()
            => await _templateService.GetAllTemplate();

        [SwaggerOperation("Get template by id")]
        [HttpGet]
        public async Task<BaseResponse> GetTemplateById(Guid id)
            => await _templateService.GetTemplateById(id);

        [SwaggerOperation("Update template")]
        [HttpPost]
        public async Task<BaseResponse> UpdateTemplate(UpdateTemplateRequest request)
            => await _templateService.UpdateTemplate(request);

        [SwaggerOperation("Sign")]
        [HttpPost]
        public async Task<BaseResponse> Sign(SignReq request, CancellationToken cancellationToken)
            => await _templateService.Sign(request, _providerSign, cancellationToken);

        [SwaggerOperation("Get sign status")]
        [HttpGet]
        public async Task<BaseResponse> GetSignStatus(string transactionId, CancellationToken cancellationToken)
            => await _templateService.GetSignStatus(transactionId, _providerSign, cancellationToken);

        [SwaggerOperation("Get file signed")]
        [HttpGet]
        public async Task<BaseResponse> GetFileSigned(string transactionId, CancellationToken cancellationToken)
            => await _templateService.GetFileSigned(transactionId, _providerSign, cancellationToken);

        [SwaggerOperation("Verify PDF Signatures")]
        [HttpPost]
        public async Task<IActionResult> VerifyPdfSignatures(VerifySignatureRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _templateService.VerifySignaturesAsync(request, _providerSign, cancellationToken));
        }

        [SwaggerOperation("Delete Template")]
        [HttpDelete]
        public async Task<BaseResponse> DeleteTemplate(Guid id)
            => await _templateService.DeleteTemplate(id);
    }
}
