using DigitalSignService.Business.IServices;
using DigitalSignService.Business.Services;
using DigitalSignService.Common;
using DigitalSignService.DAL.DTOs.Requests;
using DigitalSignService.DAL.DTOs.Responses;
using DigitalSignService.DAL.Entities;
using DigitalSignService.DAL.IRepository;
using DigitalSignService.DAL.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebGisBE.Business.Services
{
    public class PaperSizeService : BaseService<PaperSize>, IPaperSizeService
    {
        private readonly IPaperSizeRepository _rp;
        private readonly AppSetting _appSetting;
        private readonly IUserContext _userContext;
        public PaperSizeService(
            IPaperSizeRepository rp,
            IOptions<AppSetting> options,
            ILogger<TemplateService> logger,
            IUserContext userContext) : base(rp, logger)
        {
            this._rp = rp;
            _appSetting = options.Value;
            _userContext = userContext;
        }

        public async Task<BaseResponse> CustomPage(CustomPageRequest request)
        {
            try
            {
                if (request.Id.HasValue)
                {
                    var existPageSize = await _rp.Queryable().FirstOrDefaultAsync(x => x.Id == request.Id && x.CreatedUserId == _rp.UserInfo.Id && x.PaperSizeType == Enums.PaperSizeType.Custom);
                    if (existPageSize is null)
                    {
                        return BadRequestResponse("Page size does not exist");
                    }
                    existPageSize.PaperName = request.PageName;
                    existPageSize.Height = request.Height;
                    existPageSize.Width = request.Width;

                    await _rp.Update(existPageSize);
                    await _rp.Save();

                    return SuccessResponse(existPageSize.Adapt<PaperSizeDTO>(), "Create page size successfully");
                }
                else
                {
                    var existPageSize = _rp.Queryable().Any(x => x.Width == request.Width && x.Height == request.Height && x.CreatedUserId == _rp.UserInfo.Id);
                    if (existPageSize)
                    {
                        return BadRequestResponse("Exist page size");
                    }
                    var pageSize = new PaperSize
                    {
                        PaperName = request.PageName,
                        Height = request.Height,
                        Width = request.Width,
                        PaperSizeType = Enums.PaperSizeType.Custom
                    };
                    await _rp.Insert(pageSize);
                    await _rp.Save();

                    return SuccessResponse(pageSize.Adapt<PaperSizeDTO>(), "Create page size successfully");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> GetAllPaperSize()
        {
            try
            {
                var allPaperSize = await _rp.Queryable().Where(ps => ps.PaperSizeType != Enums.PaperSizeType.Custom || ps.CreatedUserId == _rp.UserInfo.Id)
                    .OrderBy(ps => ps.PaperSizeType == Enums.PaperSizeType.Custom).ThenBy(ps => ps.PaperSizeType).ToListAsync();
                if (allPaperSize == null)
                {
                    return NotFoundResponse("Not found");
                }

                return SuccessResponse(allPaperSize.Adapt<IEnumerable<PaperSizeDTO>>(), "Get all paper size successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return CatchErrorResponse(ex);
            }
        }

        public async Task<BaseResponse> DeletePaperSize(Guid id)
        {
            try
            {
                // Lấy paper size với navigation property để check relationship
                var paperSize = await _rp.FindWithIncludesAsync(
                    ps => ps.Id == id && ps.IsActive && !ps.IsDeleted
                    && ps.PaperSizeType == Enums.PaperSizeType.Custom
                    && ps.CreatedUserId == _rp.UserInfo.Id,
                    query => query.Include(ps => ps.TemplatePapers));

                if (paperSize == null)
                {
                    return NotFoundResponse("Paper size not found");
                }

                // Kiểm tra xem paper size có đang được sử dụng trong template không
                if (paperSize.TemplatePapers != null && paperSize.TemplatePapers.Any())
                {
                    return BadRequestResponse("Cannot delete paper size because it is being used in templates");
                }

                await _rp.Delete(id);
                await _rp.Save();

                return SuccessResponse(true, "Delete paper size successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return CatchErrorResponse(ex);
            }
        }
    }
}
