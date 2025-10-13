namespace DigitalSignService.DAL.DTOs.Responses
{
    public class PaperSizeDTO
    {
        public Guid Id { get; set; }
        public string PaperName { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string PaperSizeType { get; set; }
    }
}
