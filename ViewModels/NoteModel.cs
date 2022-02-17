

namespace Backend_EF.ViewModels
{
    public class NoteModel
    {
        public int Id { get; set; }//necessary id for managing in db
        public Guid IdNote { get; set; }//id for notes managing
        public string? IdCode { get; set; }//unic user`s id
        public string? Title { get; set; }
        public string? Body { get; set; }
    }
}
