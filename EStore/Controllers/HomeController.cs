﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using EStore.Models;
using EStore.Utilities;
using EStore.Utilities.DataRepository;
using PagedList;

namespace EStore.Controllers
{
    public class HomeController : Controller
    {
        static int pageSize = 8;
        public ActionResult Index(string sortBy = "All", int page = 1)
        {
            ProductDataRepository dataRepository = new ProductDataRepository();
            List<Product> productList = dataRepository.getAllProducts();
            dataRepository = new ProductDataRepository();
            ViewBag.Categories = dataRepository.getProductCategories();
            ViewBag.SortBy = sortBy;

            if (!sortBy.Equals("All"))
            {
                productList = productList.Where(p => p.Category == sortBy).ToList();
            }
                
            return View(productList.OrderBy(p => p.Name).ToPagedList(page, pageSize));
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
            if (!productDataRepository.orderProduct(cartItems, user.UserName))
                return RedirectToAction("Checkout");
            //Reset Session strings
            Session[Cart.CartCountSessionString] = 0;
            Session[Cart.CartSessionString] = new List<Cart>();

            return RedirectToAction("Index"); 
        }

        [HttpPost]
        public ActionResult DeleteCartItems(int id) 
        {
            System.Diagnostics.Debug.WriteLine("here:" +id );
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
            System.Diagnostics.Debug.WriteLine("came here");
            if (ModelState.IsValid)
            {
                UserDataRepository userDataRepository = new UserDataRepository();
                user = userDataRepository.updateUser(user);
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