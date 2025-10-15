using DigitalSignService.Business.IServices;
using DigitalSignService.Business.IServices.Sign;
using DigitalSignService.Business.Service3th;
using DigitalSignService.Common;
using DigitalSignService.DAL.DTOs.Requests;
using DigitalSignService.DAL.DTOs.Requests.Sign;
using DigitalSignService.DAL.DTOs.Responses;
using DigitalSignService.DAL.Entities;
using DigitalSignService.DAL.IRepository;
using DigitalSignService.DAL.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DigitalSignService.Business.Services
{
    public class TemplateService : BaseService<Template>, ITemplateService
    {
        private readonly ITemplateRepository _rp;
        private readonly AppSetting _appSetting;
        private readonly IUserContext _userContext;
        private readonly ISigningProviderFactory _signingProviderFactory;
        private readonly IHistorySignRepository _historySignRP;
        private readonly FileApi _fileApi;
        private readonly CachingService _cachingService;
        private readonly WebhookApi _webhookApi;
        private readonly IPaperSizeRepository _paperSizeRP;
        public TemplateService(
            ITemplateRepository rp,
            IOptions<AppSetting> options,
            ILogger<TemplateService> logger,
            IUserContext userContext,
            ISigningProviderFactory signingProviderFactory,
            IHistorySignRepository historySignRP,
            FileApi fileApi,
            CachingService cachingService,
            WebhookApi webhookApi,
            IPaperSizeRepository paperSizeRP) : base(rp, logger)
        {
            this._rp = rp;
            _appSetting = options.Value;
            _userContext = userContext;
            _signingProviderFactory = signingProviderFactory;
            _historySignRP = historySignRP;
            _fileApi = fileApi;
            _cachingService = cachingService;
            _webhookApi = webhookApi;
            _paperSizeRP = paperSizeRP;
        }

        /// <summary>
        /// Sign file pdf, currently does not support sign multiple users with priority
        /// </summary>
        /// <param name="req"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public async Task<BaseResponse> Sign(SignReq req, string providerSign, CancellationToken cancellationToken = default)
        {
            try
            {
                if (Path.GetExtension(req.DocumentInfo.Name).ToLower() != ".pdf")
                    return BadRequestResponse("Only support pdf file");

                var fileSize = await _fileApi.GetFileSizeAsync(req.DocumentInfo.Url);
                if (fileSize == -1)
                {
                    logger.LogError("Cannot get file size from document URL (use method HEAD to get file size): {DocumentUrl}", req.DocumentInfo.Url);
                    return BadRequestResponse("Cannot retrieve file information. Please check the document URL (use method HEAD to get file size).");
                }

                if (!_cachingService.CanAddToCache(fileSize, _appSetting.MaxStorageSigner))
                {
                    var currentCacheSize = _cachingService.GetAllSignerCacheSize();
                    logger.LogWarning("Cannot process document due to cache storage limit. File size: {FileSize}, Current cache: {CurrentCache}, Max allowed: {MaxStorage}",
                        fileSize, currentCacheSize, _appSetting.MaxStorageSigner);
                    return BadRequestResponse($"Cannot process document due to storage limit. File size: {fileSize / (1024 * 1024):F2}MB. Server can handle file with size {(_appSetting.MaxStorageSigner - currentCacheSize) / (1024 * 1024):F2}MB currently.");
                }

                if (req.TemplateId.HasValue)
                {
                    var existingTemplate = await _rp.FindWithIncludesAsync(t => t.Id == req.TemplateId.Value && t.IsActive && !t.IsDeleted,
                            query => query.Include(t => t.TemplatePapers));
                    if (existingTemplate is null)
                        return NotFoundResponse($"Cannot find template with id {req.TemplateId}");
                }

                var entity = new HistorySign
                {
                    DocumentUrl = req.DocumentInfo.Url,
                    DocumentId = req.DocumentInfo.Id,
                    DocumentName = req.DocumentInfo.Name,
                    UserId = req.UserSign.Id,
                    Img = req.UserSign.Img,
                    Reason = req.UserSign.Reason,
                    SerialNumber = req.UserSign.SerialNumber,
                    UserSignPositions = req.UserSign.UserSignPositions,
                    SigningStatus = Enums.SigningStatus.InQueue,
                    TemplateId = req.TemplateId,
                    Provider = providerSign
                };

                await _historySignRP.Insert(entity);
                await _historySignRP.Save();
                // Need to check if handle in global
                var _signingProvider = _signingProviderFactory.GetProvider(providerSign);
                var transactionId = await _signingProvider.SignCAPDF(req, cancellationToken);
                if (String.IsNullOrEmpty(transactionId))
                {
                    entity.SigningStatus = Enums.SigningStatus.SignFailed;
                    await _historySignRP.Update(entity);
                    await _historySignRP.Save();
                    return BadRequestResponse("Cannot create transaction to sign document");
                }
                entity.TransactionId = transactionId;
                entity.SigningStatus = Enums.SigningStatus.WaitingForUserConfirmation;

                await _historySignRP.Update(entity);
                await _historySignRP.Save();

                return SuccessResponse(entity.Id, "Sign pdf successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when sign");
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> GetSignStatus(string transactionId, string providerSign, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingHistorySign = await _historySignRP.GetActiveById(Guid.Parse(transactionId));
                if (existingHistorySign is null)
                    return NotFoundResponse($"Cannot find history sign with transaction {transactionId}");
                if (String.IsNullOrEmpty(existingHistorySign.TransactionId))
                    return BadRequestResponse($"File was not sign");

                SignReq signReq = new SignReq
                {
                    DocumentInfo = new DocumentInfo
                    {
                        Id = existingHistorySign.DocumentId,
                        Name = existingHistorySign.DocumentName,
                        Url = existingHistorySign.DocumentUrl
                    },
                    UserSign = new UserSignReq
                    {
                        Id = existingHistorySign.UserId,
                        Img = existingHistorySign.Img,
                        Reason = existingHistorySign.Reason,
                        SerialNumber = existingHistorySign.SerialNumber,
                        UserSignPositions = existingHistorySign.UserSignPositions
                    }
                };
                var _signingProvider = _signingProviderFactory.GetProvider(providerSign);
                var statusRes = await _signingProvider.GetSignStatus(signReq, existingHistorySign.TransactionId, cancellationToken);

                existingHistorySign.SigningStatus = statusRes.SigningStatus;
                existingHistorySign.DocumentSignedUrl = statusRes.DocumentUrl;

                await _historySignRP.Update(existingHistorySign);
                await _historySignRP.Save();

                return SuccessResponse(statusRes, "Get sign status successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when GetSignStatus");
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> CreateTemplate(CreateTemplateRequest request)
        {
            try
            {
                var existNameTemplate = await _rp.Queryable().AnyAsync(x => x.Name == request.Name && x.CreatedUserId == _rp.UserInfo.Id);
                if (existNameTemplate)
                {
                    return BadRequestResponse("Exist name template");
                }

                var paperIds = request.TemplatePapers.Select(x => x.PaperId).ToList().Distinct();
                var existPaperIds = await _paperSizeRP.Queryable()
                    .Where(p => paperIds.Contains(p.Id) && (p.PaperSizeType != Enums.PaperSizeType.Custom || p.CreatedUserId == _rp.UserInfo.Id))
                    .Select(p => p.Id)
                    .ToListAsync();

                var notExistPaperIds = paperIds.Except(existPaperIds).ToList();
                if (notExistPaperIds.Any())
                {
                    return BadRequestResponse($"Some page size does not exist: {String.Join(", ", notExistPaperIds)}");
                }

                var template = new Template
                {
                    Name = request.Name,
                    TemplatePapers = request.TemplatePapers.Select(p => new DigitalSignService.DAL.Entities.TemplatePaper
                    {
                        PaperSizeId = p.PaperId,
                        TemplateUrl = p.TemplateUrl,
                        TemplatePaperUserSigns = p.TemplateUserSigns.Select(u => new DigitalSignService.DAL.Entities.TemplatePaperUserSign
                        {
                            UserSignId = u.UserSignId,
                            Img = u.Img,
                            Rotate = u.Rotate,
                            Priority = u.Priority,
                            UserSignPositions = u.UserSignPos.Select(pos => new UserSignPos
                            {
                                CoorX = pos.CoorX,
                                CoorY = pos.CoorY,
                                Width = pos.Width,
                                Height = pos.Height,
                                StartPage = pos.StartPage,
                                EndPage = pos.EndPage
                            }).ToArray()
                        }).ToList()
                    }).ToList()
                };
                await _rp.Insert(template);
                await _rp.Save();

                var entity = await this._GetTemplateById(template.Id);

                return SuccessResponse(entity.Adapt<TemplateDTO>(), "Create template succesfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> GetAllTemplate()
        {
            try
            {
                var templates = await _rp.Queryable()
                    .Where(x => x.IsActive && !x.IsDeleted
                        && x.CreatedUserId == this._rp.UserInfo.Id)
                    .ToListAsync();
                var result = templates.Adapt<List<TemplateDTO>>();
                return SuccessResponse(result, "Get all template succesfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> GetTemplateById(Guid id)
        {
            try
            {
                var template = await this._GetTemplateById(id);
                if (template == null)
                    return NotFoundResponse("Template not found");

                return SuccessResponse(template.Adapt<TemplateDTO>(), "Get template succesfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> UpdateTemplate(UpdateTemplateRequest request)
        {
            try
            {
                var existTemplate = await _rp.Queryable()
                    .Include(t => t.TemplatePapers)
                        .ThenInclude(tp => tp.PaperSize)
                    .Include(t => t.TemplatePapers)
                        .ThenInclude(tpu => tpu.TemplatePaperUserSigns)
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.CreatedUserId == _rp.UserInfo.Id);
                if (existTemplate == null)
                {
                    return BadRequestResponse("Template does not exist");
                }

                var existName = await _rp.Queryable().AnyAsync(x => x.Name == request.Name && x.Id != request.Id);
                if (existName)
                {
                    return BadRequestResponse("Name template exist");
                }

                var paperIds = request.TemplatePapers.Select(x => x.PaperId).ToList().Distinct();
                var existPaperIds = await _paperSizeRP.Queryable()
                    .Where(p => paperIds.Contains(p.Id) && (p.PaperSizeType != Enums.PaperSizeType.Custom || p.CreatedUserId == _rp.UserInfo.Id))
                    .Select(p => p.Id)
                    .ToListAsync();

                var notExistPaperIds = paperIds.Except(existPaperIds).ToList();
                if (notExistPaperIds.Any())
                {
                    return BadRequestResponse($"Some page size does not exist: {String.Join(", ", notExistPaperIds)}");
                }

                var updateTemplatePapers = request.TemplatePapers.Select(p => new DigitalSignService.DAL.Entities.TemplatePaper
                {
                    PaperSizeId = p.PaperId,
                    TemplateUrl = p.TemplateUrl,
                    TemplatePaperUserSigns = p.TemplateUserSigns.Select(u => new DigitalSignService.DAL.Entities.TemplatePaperUserSign
                    {
                        UserSignId = u.UserSignId,
                        Img = u.Img,
                        Rotate = u.Rotate,
                        Priority = u.Priority,
                        UserSignPositions = u.UserSignPos.Select(pos => new UserSignPos
                        {
                            CoorX = pos.CoorX,
                            CoorY = pos.CoorY,
                            Width = pos.Width,
                            Height = pos.Height,
                            StartPage = pos.StartPage,
                            EndPage = pos.EndPage
                        }).ToArray()
                    }).ToList()
                }).ToList();
                existTemplate.Name = request.Name;
                existTemplate.TemplatePapers = updateTemplatePapers;

                await _rp.Update(existTemplate);
                await _rp.Save();

                var entity = await this._GetTemplateById(existTemplate.Id);

                return SuccessResponse(entity.Adapt<TemplateDTO>(), "Update template succesfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> GetFileSigned(string transactionId, string providerSign, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingHistorySign = await _historySignRP.GetActiveById(Guid.Parse(transactionId));
                if (existingHistorySign is null)
                    return NotFoundResponse($"Cannot find history sign with transaction {transactionId}");

                if (String.IsNullOrEmpty(existingHistorySign.DocumentSignedUrl))
                    return BadRequestResponse($"File was not sign");

                return SuccessResponse(existingHistorySign.DocumentSignedUrl, "Get file sign successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when GetFileSigned");
                return CatchErrorResponse(ex);
            }
        }

        public async Task HandleWebhook(string providerSign, string dataString, CancellationToken cancellationToken = default)
        {
            try
            {
                string transactionId = string.Empty;
                var _signingProvider = _signingProviderFactory.GetProvider(providerSign);
                switch (providerSign)
                {
                    case "viettel":
                        var data = JsonConvert.DeserializeObject<VTWebhookData>(dataString);
                        if (data is null)
                            return;
                        transactionId = data.MetaData.TransactionId;
                        break;
                    default:
                        break;
                }
                var existingHistorySign = await _historySignRP.Queryable()
                    .FirstOrDefaultAsync(hs => hs.TransactionId == transactionId && hs.SigningStatus == Enums.SigningStatus.WaitingForUserConfirmation);
                if (existingHistorySign is null)
                {
                    logger.LogWarning("No history sign found for transaction: {transactionId}", transactionId);
                    return;
                }

                var statusRes = await _signingProvider.HandleWebhook(dataString, existingHistorySign.DocumentName, cancellationToken);
                existingHistorySign.SigningStatus = statusRes.SigningStatus;
                existingHistorySign.DocumentSignedUrl = statusRes.DocumentUrl;

                await _historySignRP.UpdateWithoutTracking(existingHistorySign);
                await _historySignRP.Save();

                if (existingHistorySign.CreatedUserId.HasValue)
                    await _webhookApi.PushMessage(new PushMessReq
                    {
                        EventType = "",
                        Data = new
                        {
                            transactionId = existingHistorySign.Id,
                            status = existingHistorySign.SigningStatus,
                            documentSignedUrl = existingHistorySign.DocumentSignedUrl
                        },
                        SubscriberIds = new List<Guid> { existingHistorySign.CreatedUserId.Value }
                    });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error when HandleWebhook of provider {providerSign}");
            }
        }

        public async Task<BaseResponse> DeleteTemplate(Guid id)
        {
            try
            {
                var existTemplate = await _rp.FindWithIncludesAsync(tp => tp.Id == id
                            && tp.IsActive
                            && !tp.IsDeleted
                            && tp.CreatedUserId == _rp.UserInfo.Id, query => query.Include(tp => tp.HistorySigns));
                if (existTemplate == null)
                {
                    return BadRequestResponse("Template does not exist");
                }
                if (existTemplate.HistorySigns is not null && existTemplate.HistorySigns.Any())
                {
                    return BadRequestResponse("Cannot delete template because it is being used for sign history");
                }

                await _rp.Delete(existTemplate.Id);
                await _rp.Save();

                return SuccessResponse("Delete template succesfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return CatchErrorResponse(ex);
            }
        }

        private async Task<Template?> _GetTemplateById(Guid id)
        {
            try
            {
                return await _rp.FindWithIncludesAsync(t => t.Id == id && t.IsActive && !t.IsDeleted && t.CreatedUserId == this._rp.UserInfo.Id,
                    query => query.Include(tp => tp.TemplatePapers)
                        .ThenInclude(pp => pp.PaperSize)
                        .Include(tp => tp.TemplatePapers)
                        .ThenInclude(tpu => tpu.TemplatePaperUserSigns));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
