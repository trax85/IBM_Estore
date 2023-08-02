using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using EStore.Models;
using EStore.Utilities;
using EStore.Utilities.DataRepository;

namespace EStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string type)
        {
            //TO-DO: DUmmy data
            //products = new List<Models.Product>() 
            //{ 
            //  new Models.Product { Name = "BreadBoard", Cost = 100, Category="Electronics" },
            //  new Models.Product { Name = "Apple", Cost = 80 , Category="Grocessory"},
            //  new Models.Product { Name = "IC", Cost = 60, Category="Electronics" },
            //  new Models.Product { Name = "Orange", Cost = 120 , Category = "Grocessory"},
            //  new Models.Product { Name = "Musambi", Cost = 70 , Category = "Grocessory"},
            //};
            ProductDataRepository dataRepository = new ProductDataRepository();
            List<Product> productList = dataRepository.getAllProducts();
            dataRepository = new ProductDataRepository();
            TempData["ProductCategories"] = dataRepository.getProductCategories();
            if (type.IsEmpty() || type == "All")
                return View(productList);
            return View(productList.Where(p => p.Category == type).ToList());
        }

        public ActionResult Product(string productName)
        {
            ProductDataRepository homeDataRepository = new ProductDataRepository();
            Product product = homeDataRepository.getProduct(productName);
            if(product.Name.IsEmpty())
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
            cart.Id = cartItems.Count;
            cartItems.Add(cart);
            Session[Cart.CartCountSessionString] = cartItems.Count;
            Session[Cart.CartSessionString] = cartItems;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Checkout()
        {
            User user = Session[Models.User.UserSessionString] as User;
            if (user != null)
            {
                UserDataRepository userDataRepository = new UserDataRepository();
                user = userDataRepository.getUser(user.UserName);
                return View(user);
            } 

            return RedirectToAction("SignIn", "Login");
        }

        [HttpPost]
        public ActionResult PlaceOrder(User user)
        {
            List<Cart> cartItems = Session[Cart.CartSessionString] as List<Cart>;
            ProductDataRepository productDataRepository = new ProductDataRepository();
            productDataRepository.orderProduct(cartItems, user.UserName);
            //Reset Session strings
            Session[Cart.CartCountSessionString] = 0;
            Session[Cart.CartSessionString] = new List<Cart>();

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
            User user = Session[Models.User.UserSessionString] as User;
            System.Diagnostics.Debug.WriteLine("user:" + user.UserName);
            if (user != null)
            {
                UserDataRepository userDataRepository = new UserDataRepository();
                user = userDataRepository.getUser(user.UserName);
                return View(user);
            }
            return RedirectToAction("SignIn", "Login"); 
        }

        [HttpPost]
        public ActionResult UpdateUser(User user)
        {
            if (ModelState.IsValid)
            {
                UserDataRepository userDataRepository = new UserDataRepository();
                user = userDataRepository.updateUser(user.UserName);
                if (!user.UserName.IsEmpty())
                {
                    Session[Models.User.UserSessionString] = user;
                    return RedirectToAction("Index");
                }
            }
            return View(user);
        }

        public ActionResult ViewPurchaseHistory() 
        {
            User user = Session[Models.User.UserSessionString] as User;
            if (user != null)
            {
                CartDataRepository productDataRepository = new CartDataRepository();
                List<Cart> userCartList = productDataRepository.getPurchaseHistory(user.UserName);
                return View(userCartList);
            }
            else System.Diagnostics.Debug.Write("User not present");

            return RedirectToAction("SignIn", "Login");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}