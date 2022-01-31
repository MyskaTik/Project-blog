using Backend_EF.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend_EF.Controllers
{
    public class GameController : Controller
    {
        ApplicationContext db;
        public GameController(ApplicationContext db)
        {
            this.db = db;   
        }
        public IActionResult Index(User user)
        {
            //sets user by session
            user.Name = HttpContext.Session.GetString("userName");
            user.Email = HttpContext.Session.GetString("userEmail");
            user.Password = HttpContext.Session.GetString("userPassword");
            user.IdCode = db.GetIdCode(user.Name, user.Email, user.Password);
            //creates new object of ScoreModel to pass it in method GetScore
            user.ScoreModel = new ScoreModel()
            {
                Name = user.Name
            }; 
            //assign Score of got user
            user.ScoreModel.Score = db.GetScore(user.ScoreModel, user);
            return View(user);
        }
    }
}
