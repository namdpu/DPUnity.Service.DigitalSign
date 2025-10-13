using DigitalSignService.Common;

namespace DigitalSignService.DAL.DTOs.Responses
{
    public class SignStatusDTO
    {
        public Enums.SigningStatus SigningStatus { get; set; }
        public string? DocumentUrl { get; set; }
        public string TransactionId { get; set; }
    }
}
