using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using EStore.Models;
using EStore.Utilities.DataRepository;
using PagedList;

namespace EStore.Controllers
{
    public class HomeController : Controller
    {
        private const int pageSize = 8;
        private string redirectAction = "SignIn";
        private string redirectController = "Login";

        private readonly IProductDataRepository _productDataRepository;
        private readonly IUserDataRepository _userDataRepository;
        private readonly ITotalSalesDataRepository _totalSalesDataRepository;
        private readonly IProductDataRepositoryV2 _productDataRepositoryV2;
        private readonly IContactUsDataRepository _contactUsDataRepository;
        public HomeController(IProductDataRepository product, IUserDataRepository user, ITotalSalesDataRepository cart, 
            IProductDataRepositoryV2 productV2, IContactUsDataRepository contact) 
        {
            _productDataRepository = product;
            _userDataRepository = user;
            _totalSalesDataRepository = cart;
            _productDataRepositoryV2 = productV2;
            _contactUsDataRepository = contact;
        }
        public ActionResult Index(string sortBy = "All",string orderBy = "A-Z", int page = 1)
        {
            isUserLogged();
            List<Product> productList = _productDataRepository.GetAllProducts();
            ViewBag.Categories = _productDataRepository.GetProductCategories();
            List<string> orderList = new List<string>() {
                "A-Z",
                "Z-A",
                "$ Low-High",
                "$ High-Low"
            };
            ViewBag.SortBy = sortBy;
            ViewBag.OrderList = orderList;
            ViewBag.OrderBy = orderBy;

            if (!sortBy.Equals("All"))
            {
                productList = productList.Where(p => p.Category == sortBy).ToList();
            }

            if(orderBy == orderList[2])
            {
                productList = productList.OrderBy(p => p.Cost).ToList();
            } else if(orderBy == orderList[3])
            {
                productList = productList.OrderByDescending(p => p.Cost).ToList();
            } else if(orderBy == orderList[1])
            {
                productList = productList.OrderByDescending(p => p.Name).ToList();
            } else
            {
                productList = productList.OrderBy(p => p.Name).ToList();
            }
            
            return View(productList.ToPagedList(page, pageSize));
        }

        public ActionResult Product(string productName)
        {
            isUserLogged();
            Product product = _productDataRepository.GetProduct(productName);
            if(product.IsEmpty())
                return RedirectToAction("Index");
            return View(product);
        }

        [HttpPost]
        public ActionResult Product(Product product, int quantity) 
        {
            System.Diagnostics.Debug.WriteLine(" cart items");
            Cart cart = new Cart()
            {
                Name = product.Name,
                Category = product.Category,
                Quantity = quantity,
                Cost = product.Cost
            };
            List<Cart> cartItems = Session[Cart.CartSessionString] as List<Cart>;
            if(cartItems.Count == 0)
                cart.Id = cartItems.Count;
            else
            {
                int temp = cartItems[cartItems.Count - 1].Id;
                cart.Id = temp + 1;
            }
            cartItems.Add(cart);
            Session[Cart.CartCountSessionString] = cartItems.Count;
            Session[Cart.CartSessionString] = cartItems;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Checkout()
        {
            if (isUserLogged())
            {
                User user = Session[Models.User.UserSessionString] as User;
                user = _userDataRepository.GetUser(user.UserName);
                return View(user);
            } 

            return RedirectToAction(redirectAction, redirectController);
        }

        [HttpPost]
        public ActionResult PlaceOrder(User user, string paymentType)
        {
            List<Cart> cartItems = Session[Cart.CartSessionString] as List<Cart>;
            if(cartItems != null)
            {
                System.Diagnostics.Debug.WriteLine("Payment type:" + paymentType);
                foreach (var items in cartItems)
                {
                    items.PaymentType = paymentType;
                    if (!_productDataRepositoryV2.OrderProduct(items, user.UserName))
                        return RedirectToAction("Checkout");
                }

                //Reset Session strings
                Session[Cart.CartCountSessionString] = 0;
                Session[Cart.CartSessionString] = new List<Cart>();
            }

            return RedirectToAction("Index"); 
        }

        [HttpPost]
        public ActionResult DeleteCartItems(int id) 
        {
            List<Cart> sessionList = Session[Cart.CartSessionString] as List<Cart>;
            if (sessionList != null)
            {
                // Remove the item with the provided id from the session list
                var itemToRemove = sessionList.FirstOrDefault(item => item.Id == id);
                if (itemToRemove != null)
                {
                    sessionList.Remove(itemToRemove);
                    Session[Cart.CartSessionString] = sessionList;
                    Session[Cart.CartCountSessionString] = sessionList.Count;
                }
            }

            int total = 0;
            foreach (var item in sessionList) total += item.Quantity * item.Cost;
            int totalCount = sessionList.Count;
            return Json(new { success = true, total, totalCount });
        }

        public ActionResult UpdateUser()
        {
            if (isUserLogged())
            {
                User user = Session[Models.User.UserSessionString] as User;
                user = _userDataRepository.GetUser(user.UserName);
                return View(user);
            }
            return RedirectToAction(redirectAction, redirectController); 
        }

        [HttpPost]
        public ActionResult UpdateUser(User user)
        {
            if (ModelState.IsValid)
            {
                user = _userDataRepository.UpdateUser(user);
                if (!user.IsEmpty())
                {
                    Session[Models.User.UserSessionString] = user;
                    return RedirectToAction("Index");
                }
            }
            return View(user);
        }

        public ActionResult ViewPurchaseHistory(string dateFrom = "Start", string dateTo = "End", int page = 1) 
        {
            if (isUserLogged())
            {
                User user = Session[Models.User.UserSessionString] as User;
                List<TotalSales> userHistory = _productDataRepository.GetImagesForTotalSales(_totalSalesDataRepository.GetPurchaseHistory(user.UserName));
                ViewBag.WeekIntervals = _totalSalesDataRepository.GetWeekIntervals(userHistory);
                ViewBag.Categories = _productDataRepository.GetProductCategories();
                ViewBag.DateTo = dateTo;
                ViewBag.DateFrom = dateFrom;

                if ((!dateTo.IsEmpty() && !dateTo.Equals("End")) && (!dateFrom.IsEmpty() && !dateFrom.Equals("Start")))
                {
                    userHistory = userHistory.Where(p => p.Timestamp >= DateTime.Parse(dateFrom) && 
                        p.Timestamp <= DateTime.Parse(dateTo)).ToList();
                }

                
                return View(userHistory.OrderBy(p => p.ProductName).ToPagedList(page, pageSize - 4));
            }

            return RedirectToAction(redirectAction, redirectController);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View(_contactUsDataRepository.GetContactDetails());
        }

        public bool isUserLogged()
        {
            User user = Session[Models.User.UserSessionString] as User;
            if(user != null)
            {
                if (_userDataRepository.UpdateUserStatus(user.UserName)) return true;
            }
            return false;
        }

    }
}