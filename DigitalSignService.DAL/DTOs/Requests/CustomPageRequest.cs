namespace DigitalSignService.DAL.DTOs.Requests
{
    public class CustomPageRequest
    {
        public Guid? Id { get; set; }
        public string PageName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
