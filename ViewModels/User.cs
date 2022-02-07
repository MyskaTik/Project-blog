

namespace Backend_EF.ViewModels
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? IdCode { get; set; } = string.Empty;
        public int RoleID { get; set; } = 3;
        public string? RoleName { get; set; } = "user";//default
        public MessageModel? MessageModel { get; set; }
        public NoteModel? NoteModel { get; set; }
    }
}
