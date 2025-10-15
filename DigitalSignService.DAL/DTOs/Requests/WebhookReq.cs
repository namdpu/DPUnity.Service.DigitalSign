namespace DigitalSignService.DAL.DTOs.Requests
{
    public class PushMessReq
    {
        public string EventType { get; set; }
        public List<Guid>? SubscriberIds { get; set; }
        public object Data { get; set; }
    }
}
