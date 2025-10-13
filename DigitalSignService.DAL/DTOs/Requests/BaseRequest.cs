using DigitalSignService.Common;

namespace DigitalSignService.DAL.DTOs.Requests
{
    public class BaseRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; }
        public string[]? SortTypes { get; set; }
        public string? SearchKey { get; set; }
        public int Page { get; set; } = Constants.PageDefault;
        public int PageSize { get; set; } = Constants.PageSizeDefault;
    }
}
