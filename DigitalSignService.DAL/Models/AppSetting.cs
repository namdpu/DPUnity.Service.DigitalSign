namespace DigitalSignService.DAL.Models
{
    public class AppSetting
    {
        public string InternalKey { get; set; }
        public string BucketDigitalSign { get; set; }
        public string PublisherId { get; set; }
        public string WebHookApiKey { get; set; }
        public long MaxStorageSigner { get; set; }
    }
}
