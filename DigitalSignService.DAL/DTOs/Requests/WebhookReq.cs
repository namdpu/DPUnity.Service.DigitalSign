using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignService.DAL.DTOs.Requests
{
    public class PushMessReq
    {
        public string EventType { get; set; }
        public List<Guid>? SubscriberIds { get; set; }
        public object Data { get; set; }
    }
}
