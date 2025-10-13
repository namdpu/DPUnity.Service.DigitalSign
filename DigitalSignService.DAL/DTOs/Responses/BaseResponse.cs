namespace DigitalSignService.DAL.DTOs.Responses
{
    public class BaseResponse
    {
        public int StatusCode { get; set; } = 200;
        public string Message { get; set; } = "";
        public object? Data { get; set; }
        public string ErrorCode { get; set; } = "";
    }

    //public class AuditResponse
    //{
    //    [JsonIgnore]
    //    public UserInfo? CreateBy { get; set; }
    //    [JsonIgnore]
    //    public UserInfo? UpdateBy { get; set; }
    //    public Guid? CreatedUserId { get; set; }
    //    public Guid? UpdatedUserId { get; set; }
    //}
}
