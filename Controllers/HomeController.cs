using Microsoft.AspNetCore.Mvc;
using Backend_EF.ViewModels;

namespace Backend_EF.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        public HomeController(ApplicationContext db)
        {
            this.db = db;
        }

        //HttpGet

        [HttpGet]
        public IActionResult HomePage(MessageModel messageModel)
        {
            //sets message`s data
            messageModel.Name = HttpContext.Session.GetString("userName");
            messageModel.Email = HttpContext.Session.GetString("userEmail");
            messageModel.Password = HttpContext.Session.GetString("userPassword");
            messageModel.IdCode = db.GetIdCode(messageModel.Name, messageModel.Email, messageModel.Password);
            //sets user`s data
            User user = new()
            {
                Name = HttpContext.Session.GetString("userName"),
                Email = HttpContext.Session.GetString("userEmail"),
                Password = HttpContext.Session.GetString("userPassword")
            };
            //sets score for pass in view
            messageModel.ScoreModel = new ScoreModel()
            {
                Name = messageModel.Name
            };
            messageModel.ScoreModel.Score = db.GetScore(messageModel.ScoreModel, user);
            return View(messageModel);
        }

        [HttpGet]
        public IActionResult Chat() => View();

        [HttpGet]
        public IActionResult Error() => View();

        [HttpGet]
        public IActionResult WriteMessage(MessageModel messageModel)
        {
            messageModel.Name = HttpContext.Session.GetString("userName");
            messageModel.Email = HttpContext.Session.GetString("userEmail");
            return View(messageModel);
        }

        [HttpGet]
        public IActionResult GetAnswer(MessageModel messageModel)
        {
            messageModel.Name = HttpContext.Session.GetString("userName");
            messageModel.Email = HttpContext.Session.GetString("userEmail");
            return View(messageModel);
        }

        [HttpGet]
        public IActionResult SuccessfullySentMessage() => View();

        [HttpGet]
        public IActionResult SuccessfullyGotMessage(MessageModel messageModel)
        {
            messageModel.Name = HttpContext.Session.GetString("userName");
            messageModel.Email = HttpContext.Session.GetString("userEmail");
            return View(messageModel);
        }


        //HttpPost

        [HttpPost]
        public string Send([Bind] User user, MessageModel messageModel) => db.SendMessage(user, messageModel);

        [HttpPost]
        public IActionResult SuccessfullyGotMessage([Bind] User user, MessageModel messageModel)
        {
            messageModel.Message = db.GetMessageFromAdmin(user, messageModel);
            return View(messageModel);
        }

        [HttpPost]
        public void SendEmail(MessageModel messageModel) => db.SendMail(messageModel);
    }
}
