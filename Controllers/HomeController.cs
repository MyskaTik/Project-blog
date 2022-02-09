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
        public IActionResult HomePage(User user)
        {
            //sets user`s data
            user.Name = HttpContext.Session.GetString("userName");
            user.Email = HttpContext.Session.GetString("userEmail");
            user.Password = HttpContext.Session.GetString("userPassword");
            user.IdCode = db.GetIdCode(user);
            return View(user);
        }

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
