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
            //creates new object of ScoreModel to pass it in method GetScore
            return View(user);
        }
        

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            db.Notes.Add(user.NoteModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(User user)
        {
            return Content("Hello yopta");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(User user)
        {
            return View(user);
        }
    }
}
