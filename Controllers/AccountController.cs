using Backend_EF.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend_EF.Controllers
{
    public class AccountController : Controller
    {
        public ApplicationContext db;
        public AccountController(ApplicationContext db)
        {
            this.db = db;
        }
        public IActionResult Register() => View();
        public IActionResult Login() => View();
        public IActionResult Privacy() => View();   
        public IActionResult EnterAs() => View();
        public IActionResult ChangeUserData() => View();
        public IActionResult RecoveringPassword() => View();
        public IActionResult Recovery() => View();

        [HttpPost]
        public async Task<IActionResult> LoginUser([Bind] User receivedUser)
        {
            //setting user`s name and email into session
            HttpContext.Session.SetString("userName", receivedUser.Name);
            HttpContext.Session.SetString("userEmail", receivedUser.Email);
            HttpContext.Session.SetString("userPassword", receivedUser.Password);
            try
            {
                if (db.LoginUser(receivedUser) == "admin")
                    return RedirectToAction("EnterAs");
                else if (db.LoginUser(receivedUser) == "moderator")
                    return RedirectToAction("EnterAs");
                else if (db.LoginUser(receivedUser) == "user")
                    return RedirectToAction("HomePage", "Home");
                else
                    return RedirectToAction("Error", "Home");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([Bind] User receivedUser)
        {
            if (db.AddUser(receivedUser))
                return RedirectToAction("Login");
            else
                return RedirectToAction("PageNotFound", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserData(User receivedUser)
        {
            User changeableUser = new()
            {
                Name = HttpContext.Session.GetString("userName"),
                Email = HttpContext.Session.GetString("userEmail"),
                Password = HttpContext.Session.GetString("userPassword")
            };
            changeableUser.IdCode = db.GetIdCode(changeableUser);
            db.EditUserData(changeableUser, receivedUser);    
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Recovery([Bind] User receivedUser)
        {
            if (db.IsExistWithoutPassword(receivedUser))
            {
                if (db.IsExistWithIdCode(receivedUser))
                {
                    receivedUser.Password = db.GetPassword(receivedUser.IdCode);
                    return View(receivedUser);
                }
                else
                    return View("name, email or id code is wrong.");
            }
            else
                return Content("user isn`t exist.");
            
        }
    }
}
