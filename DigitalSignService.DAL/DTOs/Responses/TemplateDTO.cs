using DigitalSignService.DAL.Models;

namespace DigitalSignService.DAL.DTOs.Responses
{
    public class TemplateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<TemplatePaperDTO> TemplatePapers { get; set; }
    }
    public class TemplatePaperDTO
    {
        public Guid Id { get; set; }
        public PaperSizeDTO PaperSize { get; set; }
        public List<TemplateUserSignPosDTO> TemplatePaperUserSigns { get; set; }
    }
    public class TemplateUserSignPosDTO
    {
        public Guid Id { get; set; }
        public string UserSignId { get; set; }
        public int Priority { get; set; }
        public string Img { get; set; }
        public List<UserSignPos> UserSignPositions { get; set; }
    }
}
