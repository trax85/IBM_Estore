using System.Web.Mvc;
using EStore.Models;
using EStore.Utilities;
using System.Web.WebPages;

namespace EStore.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult SignIn() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(UserLoginCredentials userCred)
        {
            UserDataRepository loginDataRepository = new UserDataRepository();
            User user = loginDataRepository.VerifyUser(userCred);

            if (!user.Type.IsEmpty())
            {
                HttpContext.Session[Models.User.UserSessionString] = user;
                if (user.Type == Models.User.UserTypes.Customer.ToString())
                    return RedirectToAction("Index", "Home");
                else
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please enter correct username/password";
                System.Diagnostics.Debug.WriteLine("Please enter correct username/password");
            }

            return View(userCred);  
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                UserDataRepository loginDataRepository = new UserDataRepository();
                user.Type = Models.User.UserTypes.Customer.ToString();
                if (loginDataRepository.CreateUser(user))
                {
                    TempData.Remove("ErrorMessage");
                    return RedirectToAction("SignIn", "Login");
                }
                System.Diagnostics.Debug.WriteLine("Could not register user");
                TempData["ErrorMessage"] = "Username already exists choose a different username";
            }

            return View(user);
        }

        public ActionResult Logout() 
        {
            Session.Remove(Models.User.UserSessionString);
            return RedirectToAction("Index", "Home");
        }
    }
}