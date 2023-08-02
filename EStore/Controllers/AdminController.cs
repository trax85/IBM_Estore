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
            User user = Session[Models.User.UserSessionString] as User;
            System.Diagnostics.Debug.WriteLine("Dashboard");
            if(user != null)
            {
                if(user.Type == Models.User.UserTypes.Admin.ToString())
                {
                    AdminDashboard dashboard = new AdminDashboard();
                    if (HttpContext.Application["UserCount"] != null)
                    {
                        dashboard.TotalLoggedUsers = (int)HttpContext.Application["UserCount"];
                    }
                    else
                    {
                        dashboard.TotalLoggedUsers = 0;
                    }

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

        public ActionResult Users()
        {
            List<User> userList = new List<User>();
            UserDataRepository dataRepository = new UserDataRepository();
            userList = dataRepository.getAllUsers();
            return View(userList);
        }

        public ActionResult CreateUser()
        {
            TempData["isEdit"] = "false";
            User user = new User();
            return View("CreateEditUser", user);
        }

        public ActionResult EditUser(string userid)
        {
            TempData["isEdit"] = "true";
            UserDataRepository dataRepository = new UserDataRepository();
            User user = dataRepository.getUser(userid);
            if(!user.UserName.IsEmpty())
                return View("CreateEditUser", user);

            return RedirectToAction("Users");
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
        {
            UserDataRepository dataRepository = new UserDataRepository();
            dataRepository.deleteUser(userId);

            return RedirectToAction("Users");
        }

        public ActionResult Products()
        {
            ProductDataRepository dataRepository = new ProductDataRepository();
            List<Product> productList = dataRepository.getAllProducts();
            
            return View(productList);
        }

        public ActionResult CreateProduct()
        {
            // TO-DO: get products from db 
            TempData["isEdit"] = "false";
            ProductDataRepository dataRepository = new ProductDataRepository();
            Product product = new Product()
            {
                ProductCategories = dataRepository.getProductCategories(),
            };

            return View("CreateEditProduct", product);
        }

        public ActionResult EditProduct(string productId)
        {
            
            ProductDataRepository dataRepository = new ProductDataRepository();
            Product product = dataRepository.getProduct(productId);
            dataRepository = new ProductDataRepository();
            product.ProductCategories = dataRepository.getProductCategories();
            if (!product.Name.IsEmpty())
            {
                TempData["isEdit"] = "true";
                return View("CreateEditProduct", product);
            }
                
            return RedirectToAction("Products");
        }

        public ActionResult DeleteProduct(string productId)
        {
            ProductDataRepository dataRepository = new ProductDataRepository();
            dataRepository.deleteProduct(productId);

            return RedirectToAction("Products");
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
    }
}