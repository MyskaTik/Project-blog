using Backend.Helpers;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace Backend_EF.Controllers
{
    
    public class AdminController : Controller
    {
        public IActionResult Index(User user) => View();

        [HttpPost]
        public string Messages([Bind] MessageModel messageModel) => Sending.GetMessage(messageModel);

        [HttpPost]
        public string AllUsers([Bind] User user) => Sending.GetUser(user);

        [HttpPost]
        public string DeleteMessage([Bind] User user) => Sending.DeleteMessage(user, Entrance.QUERYCONNECTION);

        [HttpPost]
        public string SendMessageFromAdmin([Bind] User user, MessageModel messageModel) => Sending.SendMessageFromAdmin(user, messageModel);
    }
}
