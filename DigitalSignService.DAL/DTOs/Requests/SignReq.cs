using DigitalSignService.DAL.Models;

namespace DigitalSignService.DAL.DTOs.Requests
{
    public class SignReq
    {
        public Guid? TemplateId { get; set; }
        public UserSignReq UserSign { get; set; }
        public DocumentInfo DocumentInfo { get; set; }
    }

    public class UserSignReq
    {
        public string Id { get; set; }
        public string Img { get; set; }
        public UserSignPos[] UserSignPositions { get; set; }
        public string Reason { get; set; }
        public string? SerialNumber { get; set; }
        public int? Rotate { get; set; }
    }

    public class DocumentInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class UserSignDetail : UserSignPos
    {
        public string Id { get; set; }
        public string Reason { get; set; }
        public string? SerialNumber { get; set; }

        public UserSignDetail(string id, string reason, string? serialNumber, UserSignPos userSignPos)
        {
            this.CoorY = userSignPos.CoorY;
            this.CoorX = userSignPos.CoorX;
            this.Height = userSignPos.Height;
            this.Width = userSignPos.Width;
            this.StartPage = userSignPos.StartPage;
            this.EndPage = userSignPos.EndPage;
            this.Id = id;
            this.Reason = reason;
            this.SerialNumber = serialNumber;
        }
    }
}
