﻿using System.Web.Mvc;
using EStore.Models;
using EStore.Utilities;
using System.Web.WebPages;
using EStore.Utilities.DataRepository;

namespace EStore.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserDataRepository _userDataRepository;
        public LoginController(IUserDataRepository user) 
        { 
            _userDataRepository = user;
        }
        public ActionResult SignIn() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(UserLoginCredentials userCred)
        {
            User user = _userDataRepository.VerifyUser(userCred);

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
                user.Type = Models.User.UserTypes.Customer.ToString();
                //TO-DO: Validation -> model
                if(user.UserName.Length < 6)
                {
                    ViewBag.Error = "Username must be greater than 5 letters";
                    return View(user);
                }
                else if (_userDataRepository.CreateUser(user))
                {
                    return RedirectToAction("SignIn", "Login");
                }
                ViewBag.Error = "Username already exists choose a different username";
            }

            return View(user);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string emailAddress)
        {
            if(_userDataRepository.VerifyUser(emailAddress))
            {
                TokenGenerator tokenGenerator = new TokenGenerator();
                var token = tokenGenerator.GenerateToken(emailAddress);
                if (_userDataRepository.UpdateUserToken(token, emailAddress))
                {
                    var passwordLink = Url.Action("ResetPassword", "Login", 
                        new { email=emailAddress, token = token });
                    System.Diagnostics.Debug.WriteLine(passwordLink);
                    ViewBag.Verfied = "Mail has been sent to you email Inbox please look into that for further instructions.";
                    return View();
                }
                else ViewBag.Error = "Server encountered error please try again.";
            }
            ViewBag.Error = "No known user is found using the email address provided.";
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(string email, string token)
        {
            User user = _userDataRepository.VerifyUserToken(token, email);
            if (!user.UserName.IsEmpty())
            {
                return View(user);
            }
            TempData["error"] = "Link expired/Wrong url";
            return View(new User());
        }

        [HttpPost]
        public ActionResult ResetPassword(User user)
        {
            if (ModelState.IsValid)
            {
                User temp = _userDataRepository.GetUser(user.UserName);
                if (!temp.UserName.IsEmpty())
                {
                    if (temp.Password != user.Password)
                    {
                        temp = _userDataRepository.UpdateUser(user);
                        if (!temp.UserName.IsEmpty()) return RedirectToAction("SignIn");
                    }
                    else
                    {
                        ViewBag.Error = "Password same as before, please enter a diffrent password";
                        return View(user);
                    }
                }
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