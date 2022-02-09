
namespace Backend_EF.ViewModels
{
    public class MessageModel
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? ToEmail { get; set; } = "maximkirichenk0.06@gmail.com";
        public string? Message { get; set; } = string.Empty;
    }
}
