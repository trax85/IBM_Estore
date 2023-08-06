using System.Collections.Generic;
using System.Web.Mvc;
using EStore.Models;
using EStore.Utilities.DataRepository;
using EStore.Utilities;
using System.Web.WebPages;
using System.Linq;
using System;
using PagedList;

namespace EStore.Controllers
{
    public class AdminController : Controller
    {
        static int pageSize = 2;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IProductDataRepository _productDataRepository;
        private readonly ITotalSalesDataRepository _totalSalesDataRepository;
        public AdminController(IUserDataRepository user, IProductDataRepository product, ITotalSalesDataRepository sales) 
        {
            _userDataRepository = user;
            _productDataRepository = product;
            _totalSalesDataRepository = sales;
        }
        // GET: Admin
        public ActionResult DashBoard()
        {
            if (isAuthorized())
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
                dashboard = dataRepository.GetDashBoardCardData();
                dataRepository = new DashboardDataRepository();
                AdminDashboard tempDashboard = dataRepository.GetAdminDashboardTable();
                dashboard.ProductCategories = tempDashboard.ProductCategories;
                dashboard.CategoryCount = tempDashboard.CategoryCount;
                dashboard.TotalCost = tempDashboard.TotalCost;

                return View(dashboard);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Users(string sortBy = "All", int page = 1)
        {
            if (isAuthorized())
            {
                List<User> userList = new List<User>();
                UserDataRepository dataRepository = new UserDataRepository();
                userList = dataRepository.GetAllUsers();
                System.Diagnostics.Debug.WriteLine("sort:" + sortBy);
                ViewBag.sortBy = sortBy;

                if (!sortBy.Equals("All"))
                {
                    ViewBag.sortBy = sortBy;
                    userList = userList.Where(p => p.Type == sortBy).ToList();
                }
                return View(userList.OrderBy(p => p.FirstName).ToPagedList(page, pageSize));
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CreateUser()
        {
            if (isAuthorized())
            {
                TempData["isEdit"] = "false";
                User user = new User();
                return View("CreateEditUser", user);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EditUser(string userid)
        {
            if (isAuthorized())
            {
                TempData["isEdit"] = "true";
                UserDataRepository dataRepository = new UserDataRepository();
                User user = dataRepository.GetUser(userid);
                if (!user.UserName.IsEmpty())
                    return View("CreateEditUser", user);

                return RedirectToAction("Users");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Save(User user, bool isEdit)
        {
            if (ModelState.IsValid)
            {
                UserDataRepository dataRepository = new UserDataRepository();
                if (isEdit)
                {
                    user = dataRepository.UpdateUser(user);
                    if (!user.UserName.IsEmpty()) return RedirectToAction("Users");
                }
                else
                {
                    if (dataRepository.CreateUser(user)) return RedirectToAction("Users");
                }

            }
            if (isEdit) TempData["isEdit"] = "true";
            else TempData["isEdit"] = "false";

            return View("CreateEditUser", user);
        }

        public ActionResult DeleteUser(string userId)
        {
            if (isAuthorized())
            {
                UserDataRepository dataRepository = new UserDataRepository();
                dataRepository.DeleteUser(userId);

                return RedirectToAction("Users");
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Products(string sortBy = "All", int page = 1)
        {
            if (isAuthorized())
            {
                ProductDataRepository dataRepository = new ProductDataRepository();
                List<Product> productList = dataRepository.GetAllProducts();
                dataRepository = new ProductDataRepository();
                List<string> categories = dataRepository.GetProductCategories();
                ViewBag.Categories = categories;
                ViewBag.sortBy = sortBy;

                if (!sortBy.Equals("All"))
                {
                    productList = productList.Where(p => p.Category == sortBy).ToList();
                }                 
                return View(productList.OrderBy(p => p.Name).ToPagedList(page, pageSize));
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CreateProduct()
        {
            if (isAuthorized())
            {
                // TO-DO: get products from db 
                TempData["isEdit"] = "false";
                ProductDataRepository dataRepository = new ProductDataRepository();
                Product product = new Product()
                {
                    ProductCategories = dataRepository.GetProductCategories(),
                };

                return View("CreateEditProduct", product);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EditProduct(string productId)
        {
            if (isAuthorized())
            {
                ProductDataRepository dataRepository = new ProductDataRepository();
                Product product = dataRepository.GetProduct(productId);
                dataRepository = new ProductDataRepository();
                product.ProductCategories = dataRepository.GetProductCategories();
                if (!product.Name.IsEmpty())
                {
                    TempData["isEdit"] = "true";
                    return View("CreateEditProduct", product);
                }

                return RedirectToAction("Products");
            }

            return RedirectToAction("Index", "Home"); 
        }

        public ActionResult DeleteProduct(string productId)
        {
            if (isAuthorized())
            {
                ProductDataRepository dataRepository = new ProductDataRepository();
                dataRepository.DeleteProduct(productId);

                return RedirectToAction("Products");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SaveProduct(Product product, bool isEdit)
        {
            if (ModelState.IsValid)
            {
                ProductDataRepository dataRepository = new ProductDataRepository();
                if (isEdit)
                {
                    if (dataRepository.EditProduct(product))
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
                    if (dataRepository.CreateProduct(product))
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