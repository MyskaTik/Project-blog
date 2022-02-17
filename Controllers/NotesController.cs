using Backend_EF.HtmlHelpers;
using Backend_EF.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend_EF.Controllers
{
    public class NotesController : Controller
    {
        ApplicationContext db;
        public NotesController(ApplicationContext db)
        {
            this.db = db;
        }

        public IActionResult Index(User user)
        {
            //sets user by session
            user.Name = HttpContext.Session.GetString("userName");
            user.Email = HttpContext.Session.GetString("userEmail");
            user.Password = HttpContext.Session.GetString("userPassword");
            user.IdCode = db.GetIdCode(user);
            return View(user);
        }

        public IActionResult CreateNote(User user)
        {
            user.Name = HttpContext.Session.GetString("userName");
            user.Email = HttpContext.Session.GetString("userEmail");
            user.Password = HttpContext.Session.GetString("userPassword");
            user.IdCode = db.GetIdCode(user);
            return View(user);
        }

        [HttpGet]
        public IActionResult EditNote(User user, Guid IdNote)
        {
            user.Name = HttpContext.Session.GetString("userName");
            user.Email = HttpContext.Session.GetString("userEmail");
            user.Password = HttpContext.Session.GetString("userPassword");
            user.IdCode = db.GetIdCode(user);
            user.NoteModel = new NoteModel()
            {
                Title = NoteHandler.GetNoteTitleByIdNote(IdNote),
                Body = NoteHandler.GetNoteBodyByIdNote(IdNote)
            };
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditNote(User user, Guid IdNote, string[] args)
        {
            user.Name = HttpContext.Session.GetString("userName");
            user.Email = HttpContext.Session.GetString("userEmail");
            user.Password = HttpContext.Session.GetString("userPassword");
            user.IdCode = db.GetIdCode(user);
            db.EditNote(user.NoteModel, IdNote, user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNote(Guid IdNote)
        {
            NoteModel deleteNoteModel = new NoteModel()
            {
                IdNote = IdNote
            };
            User user = new User()
            {
                Name = HttpContext.Session.GetString("userName"),
                Email = HttpContext.Session.GetString("userEmail"),
                Password = HttpContext.Session.GetString("userPassword")
            };
            user.IdCode = db.GetIdCode(user);
            await db.DeleteNoteAsync(deleteNoteModel);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote(User user, string[] args)
        {
            user.Name = HttpContext.Session.GetString("userName");
            user.Email = HttpContext.Session.GetString("userEmail");
            user.Password = HttpContext.Session.GetString("userPassword");
            user.IdCode = db.GetIdCode(user);
            db.CreateNote(user.NoteModel, user);
            return RedirectToAction("Index");
        }
    }
}
