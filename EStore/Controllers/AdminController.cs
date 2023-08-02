using System.Collections.Generic;
using System.Web.Mvc;
using EStore.Models;
using EStore.Utilities.DataRepository;
using EStore.Utilities;
using System.Web.WebPages;

namespace EStore.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult DashBoard()
        {
            if (isAuthorized())
            {

                    DashboardDataRepository dataRepository = new DashboardDataRepository();
                    dashboard = dataRepository.getDashBoardCardData();
                    dataRepository = new DashboardDataRepository();
                    AdminDashboard tempDashboard = dataRepository.GetAdminDashboardTable();
                    dashboard.ProductCategories = tempDashboard.ProductCategories;
                    dashboard.CategoryCount = tempDashboard.CategoryCount;
                    dashboard.TotalCost = tempDashboard.TotalCost;

                    return View(dashboard);
            }
            }
            return RedirectToAction("SignIn", "Login");
        }

            if (isAuthorized())
            {
            }
        }

        public ActionResult CreateUser()
            if (isAuthorized())
            {
            }

        public ActionResult EditUser(string userid)
            if (isAuthorized())
            {

            }

        [HttpPost]
        public ActionResult Save(User user, bool isEdit)
        {
            if (ModelState.IsValid)
            {
                UserDataRepository dataRepository = new UserDataRepository();
                if (isEdit)
                {
                    user = dataRepository.updateUser(user.UserName);
                    if(!user.UserName.IsEmpty()) return RedirectToAction("Users");
                }
                else
                {
                    if (dataRepository.createUser(user)) return RedirectToAction("Users");
                } 
                
            }
            if (isEdit) TempData["isEdit"] = "true";
            else TempData["isEdit"] = "false";

            return View("CreateEditUser", user);
        }

        public ActionResult DeleteUser(string userId)
            if (isAuthorized())
            {

            return RedirectToAction("Users");
            }
            if (isAuthorized())
            {
            }
        }

        public ActionResult CreateProduct()
            if (isAuthorized())
            {

            }

        public ActionResult EditProduct(string productId)
        {
            if (isAuthorized())
            {
            }

        public ActionResult DeleteProduct(string productId)
            if (isAuthorized())
            {

            }

        [HttpPost]
        public ActionResult SaveProduct(Product product, bool isEdit)
        {
            if (ModelState.IsValid)
            {
                ProductDataRepository dataRepository = new ProductDataRepository();
                if (isEdit)
                {
                    if (dataRepository.editProduct(product))
                    {
                        return RedirectToAction("Products");
                    }
                    else
                    {
                        TempData["isEdit"] = "true";
                        TempData["ErrorMessage"] = "Could not update product try again";
                    }
                }
                else
                {
                    if (dataRepository.createProduct(product))
                    {
                        return RedirectToAction("Products");
                    }
                    else
                    {
                        TempData["isEdit"] = "false";
                        TempData["ErrorMessage"] = "Could not create product try again";
                    }
                }
            }
            return View("CreateEditProduct",product);
        }
        public bool isAuthorized() 
        {
            User user = Session[Models.User.UserSessionString] as User;
            if(user != null)
            {
                if (user.Type == Models.User.UserTypes.Admin.ToString())
                    return true;
            }

            return false;
        }
    }
}