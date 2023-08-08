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
        static int pageSize = 4;
        private readonly string RedirectAction = "Index";
        private readonly string ReadirectController = "Home";
        private readonly string CreateEditUser = "CreateEditUser";
        private readonly IUserDataRepository _userDataRepository;
        private readonly IProductDataRepository _productDataRepository;
        private readonly ITotalSalesDataRepository _totalSalesDataRepository;
        private readonly IContactUsDataRepository _contactUsDataRepository;
        public AdminController(IUserDataRepository user, IProductDataRepository product, 
            ITotalSalesDataRepository sales, IContactUsDataRepository contact) 
        {
            _userDataRepository = user;
            _productDataRepository = product;
            _totalSalesDataRepository = sales;
            _contactUsDataRepository = contact;
        }
        // GET: Admin
        public ActionResult DashBoard()
        {
            if (isAuthorized())
            {
                AdminDashboard dashboard = new AdminDashboard();

                DashboardDataRepository dataRepository = new DashboardDataRepository();
                dashboard = dataRepository.GetDashBoardCardData();
                dashboard.TotalLoggedUsers = _userDataRepository.GetActiveUserCount();
                dataRepository = new DashboardDataRepository();
                dashboard = dataRepository.GetAdminDashboardTable(dashboard);

                return View(dashboard);
            }

            return RedirectToAction(RedirectAction, ReadirectController);
        }

        public ActionResult Users(string sortBy = "All", int page = 1)
        {
            if (isAuthorized())
            {
                List<User> userList = new List<User>();
                UserDataRepository dataRepository = new UserDataRepository();
                userList = dataRepository.GetAllUsers();
                ViewBag.sortBy = sortBy;

                if (!sortBy.Equals("All"))
                {
                    ViewBag.sortBy = sortBy;
                    userList = userList.Where(p => p.Type == sortBy).ToList();
                }
                return View(userList.OrderBy(p => p.FirstName).ToPagedList(page, pageSize));
            }
            return RedirectToAction(RedirectAction, ReadirectController);
        }

        public ActionResult CreateUser()
        {
            if (isAuthorized())
            {
                ViewBag.isEdit = false;
                User user = new User();
                return View(CreateEditUser, user);
            }
            return RedirectToAction(RedirectAction, ReadirectController);
        }

        public ActionResult EditUser(string userid)
        {
            if (isAuthorized())
            {
                ViewBag.isEdit = true;
                User user = _userDataRepository.GetUser(userid);
                if (!user.IsEmpty())
                    return View(CreateEditUser, user);

                return RedirectToAction("Users");
            }
            return RedirectToAction(RedirectAction, ReadirectController);
        }

        [HttpPost]
        public ActionResult Save(User user, bool isEdit)
        {
            if (ModelState.IsValid)
            {
                if (isEdit)
                {
                    user = _userDataRepository.UpdateUser(user);
                    if (!user.IsEmpty()) return RedirectToAction("Users");
                    else ViewBag.Error = "Failed to update user data, try again";
                }
                else
                {
                    if (_userDataRepository.CreateUser(user)) return RedirectToAction("Users");
                    else ViewBag.Error = "Username already taken";
                }

            }
            ViewBag.isEdit = isEdit;

            return View(CreateEditUser, user);
        }

        public ActionResult DeleteUser(string userId)
        {
            if (isAuthorized())
            {
                _userDataRepository.DeleteUser(userId);

                return RedirectToAction("Users");
            }
            return RedirectToAction(RedirectAction, ReadirectController);
        }

        public ActionResult Products(string sortBy = "All", int page = 1)
        {
            if (isAuthorized())
            {
                List<Product> productList = _productDataRepository.GetAllProducts();
                List<string> categories = _productDataRepository.GetProductCategories();
                ViewBag.Categories = categories;
                ViewBag.sortBy = sortBy;

                if (!sortBy.Equals("All"))
                {
                    productList = productList.Where(p => p.Category == sortBy).ToList();
                }                 
                return View(productList.OrderBy(p => p.Name).ToPagedList(page, pageSize));
            }
            return RedirectToAction(RedirectAction, ReadirectController);
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
            return RedirectToAction(RedirectAction, ReadirectController);
        }

        public ActionResult EditProduct(string productId)
        {
            if (isAuthorized())
            {
                Product product = _productDataRepository.GetProduct(productId);
                product.ProductCategories = _productDataRepository.GetProductCategories();
                {
                    TempData["isEdit"] = "true";
                    return View("CreateEditProduct", product);
                }

                return RedirectToAction("Products");
            }

            return RedirectToAction(RedirectAction, ReadirectController); 
        }

        public ActionResult DeleteProduct(string productId)
        {
            if (isAuthorized())
            {
                _productDataRepository.DeleteProduct(productId);

                return RedirectToAction("Products");
            }
            return RedirectToAction(RedirectAction, ReadirectController);
        }

        [HttpPost]
        public ActionResult SaveProduct(Product product, bool isEdit)
        {
            if (ModelState.IsValid)
            {
                if (isEdit)
                {
                    if (_productDataRepository.EditProduct(product))
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
                    if (_productDataRepository.CreateProduct(product))
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

        public ActionResult SalesHistory(string sortBy = "All", string dateFrom = "Start", string dateTo = "End", int page = 1)
        {
            if (isAuthorized())
            {
                List<TotalSales> salesList = _productDataRepository.MapProductToCategory(_totalSalesDataRepository.GetAllPurchaseHistory());
    
                ViewBag.WeekIntervals = _totalSalesDataRepository.GetWeekIntervals(salesList);
                ViewBag.Categories = _productDataRepository.GetProductCategories();
                ViewBag.DateTo = dateTo;
                ViewBag.DateFrom = dateFrom;
                ViewBag.sortBy = sortBy;

                if ((!dateTo.IsEmpty() && !dateTo.Equals("End")) && (!dateFrom.IsEmpty() && !dateFrom.Equals("Start")))
                {
                    salesList = salesList.Where(p => p.Timestamp >= DateTime.Parse(dateFrom) &&
                        p.Timestamp <= DateTime.Parse(dateTo)).ToList();
                }
                if (!sortBy.Equals("All"))
                {
                    salesList = salesList.Where(p => p.Category == sortBy).ToList();
                }

                return View(salesList.OrderBy(p => p.ProductName).ToPagedList(page, pageSize));
            }
            return RedirectToAction(RedirectAction, ReadirectController);
        }

        public ActionResult ContactInfo()
        {
            if (isAuthorized())
            {
                ContactUs contact = _contactUsDataRepository.GetContactDetails();
                return View(contact);
            }

            return RedirectToAction(RedirectAction, ReadirectController);
        }

        [HttpPost]
        public ActionResult ContactInfo(ContactUs contact)
        {
            if (ModelState.IsValid)
            {
                if (_contactUsDataRepository.UpdateContactDetails(contact)) 
                    return RedirectToAction("Dashboard");
            }

            return View(contact);
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