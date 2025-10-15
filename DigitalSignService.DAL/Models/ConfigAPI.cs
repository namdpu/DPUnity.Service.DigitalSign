namespace DigitalSignService.DAL.Models
{
    public class ConfigAPI
    {
        public ViettelAPI ViettelAPI { get; set; }
        public VnptAPI VnptAPI { get; set; }
        public WebHookAPI WebHookAPI { get; set; }
    }

    public abstract class BaseAPI
    {
        public string Endpoint { get; set; }
    }

    public class ViettelAPI : BaseAPI
    {
        public string Login { get; set; }
        public string GetCredentials { get; set; }
        public string SignHash { get; set; }
        public string GetSignStatus { get; set; }
    }

    public class VnptAPI : BaseAPI
    {
        public string Login { get; set; }
    }

    public class WebHookAPI : BaseAPI
    {
        public string PushMessage { get; set; }
    }
}
