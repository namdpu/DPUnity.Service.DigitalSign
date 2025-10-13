using DigitalSignService.Business.IServices;
using DigitalSignService.Common;
using DigitalSignService.DAL.DTOs.Responses;
using DigitalSignService.DAL.IRepository;
using Microsoft.Extensions.Logging;

namespace DigitalSignService.Business.Services
{
    public abstract class BaseService<T> : GenericService<T>, IBaseService<T> where T : class
    {
        protected readonly ILogger logger;
        protected BaseService(IBaseRepository<T> rpBase, ILogger logger) : base(rpBase)
        {
            this.logger = logger;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // các đối tượng có Dispose gọi ở đây
                //if (context != null) context.Dispose();
            }
        }
        ~BaseService()
        {
            Dispose();
        }

        protected BaseResponse DefineResponse(int statusCode, string message, object data)
        {
            return new BaseResponse()
            {
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
        }

        protected BaseResponse SuccessResponse(object data, string message = "")
        {
            return new BaseResponse()
            {
                StatusCode = 200,
                Data = data,
                Message = message,
            };
        }

        protected BaseResponse DataNullResponse()
        {
            return new BaseResponse()
            {
                StatusCode = 404,
                Message = Constants.DATA_NULL,
            };
        }

        protected BaseResponse BadRequestResponse(string message, string errorCode = "")
        {
            return new BaseResponse()
            {
                StatusCode = 400,
                Message = message,
                ErrorCode = errorCode,
            };
        }

        protected BaseResponse NotFoundResponse(string message)
        {
            return new BaseResponse()
            {
                StatusCode = 404,
                Message = message,
            };
        }

        protected BaseResponse CatchErrorResponse(Exception e)
        {
            return new BaseResponse()
            {
                StatusCode = 500,
                Message = $"{Constants.ERROR_NOTI}: {e.Message}",
            };
        }

        protected BaseResponse ForbiddenResponse(string? message = null)
        {
            return new BaseResponse()
            {
                StatusCode = 403,
                Message = message ?? "You do not have permission!",
            };
        }
    }
}
