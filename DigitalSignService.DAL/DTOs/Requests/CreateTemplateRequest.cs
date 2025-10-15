using DigitalSignService.DAL.Models;

namespace DigitalSignService.DAL.DTOs.Requests
{
    public class CreateTemplateRequest
    {
        public string Name { get; set; }
        public List<TemplatePaper> TemplatePapers { get; set; }
    }
    public class TemplatePaper
    {
        public Guid PaperId { get; set; }
        public string TemplateUrl { get; set; }
        public List<TemplateUsserSignPos> TemplateUserSigns { get; set; }
    }
    public class TemplateUsserSignPos
    {
        public string UserSignId { get; set; }
        public string Img { get; set; }
        public int? Rotate { get; set; }
        public int Priority { get; set; }
        public List<UserSignPos> UserSignPos { get; set; }
    }
    public class UpdateTemplateRequest : CreateTemplateRequest
    {
        public Guid Id { get; set; }
    }
}
