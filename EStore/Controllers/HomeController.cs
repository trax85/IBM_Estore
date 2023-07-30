using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using EStore.Models;

namespace EStore.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MSSQLServer"].ConnectionString;
        List<Product> products = new List<Product>();
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
            if(products.Any() == false)
            {
                string query = "SELECT * FROM products;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while(reader.Read())
                                {
                                    Product product = new Product()
                                    {
                                        Name = reader["name"].ToString(),
                                        Category = reader["category"].ToString(),
                                        Cost = reader["cost"].GetHashCode(),
                                    };
                                    products.Add(product);
                                }
                            }
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
            }
            if (type.IsEmpty() || type == "All")
                return View(products);
            return View(products.Where(p => p.Category == type).ToList());
        }

        public ActionResult Product(string productName)
        {
            Product product = new Product();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("getProductByName", connection))
            {
                try
                {
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = productName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            product.Name = reader["name"].ToString();
                            product.Description = reader["description"].ToString();
                            product.AdditionalDescription = reader["additional_description"].ToString();
                            product.Cost = reader["cost"].GetHashCode();
                            product.Category = reader["category"].ToString();
                            return View(product);
                        }
                    }
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

            return RedirectToAction("Index");
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
                user = getUserData(user);
                return View(user);
            } 

            return RedirectToAction("SignIn", "Login");
        }

        [HttpPost]
        public ActionResult PlaceOrder(User user)
        {
            List<Cart> cartItems = Session[Cart.CartSessionString] as List<Cart>;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                foreach (var item in cartItems)
                {
                    using (SqlCommand cmd = new SqlCommand("orderProduct", connection))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = user.UserName;
                            cmd.Parameters.Add("@product_name", SqlDbType.VarChar).Value = item.Name;
                            cmd.Parameters.Add("@quantity", SqlDbType.VarChar).Value = item.Quantity;
                            cmd.Parameters.Add("@cost", SqlDbType.VarChar).Value = item.Cost;
                            connection.Open();
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Write(ex.ToString());
                        }
                    }
                }
            }
            //Reset Session strings
            Session[Cart.CartCountSessionString] = 0;
            Session[Cart.CartSessionString] = new List<Cart>();

            return RedirectToAction("Index"); 
        }

        [HttpPost]
        public ActionResult DeleteCartItems(int id) 
        {
            System.Diagnostics.Debug.WriteLine("Items to be deleted:" + id);
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
            System.Diagnostics.Debug.WriteLine(total);
            int totalCount = sessionList.Count;
            return Json(new { success = true, total, totalCount });
        }

        public ActionResult UpdateUser()
        {
            System.Diagnostics.Debug.WriteLine("Update User");
            User user = Session[Models.User.UserSessionString] as User;
            if (user != null)
            {
                user = getUserData(user);
                return View(user);
            }
            return RedirectToAction("SignIn", "Login"); 
        }

        [HttpPost]
        public ActionResult UpdateUser(User user)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("updateUser", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@firstname", SqlDbType.VarChar).Value = user.FirstName;
                    cmd.Parameters.Add("@lastname", SqlDbType.VarChar).Value = user.LastName;
                    cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = user.UserName;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = user.Password;
                    cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = user.EmailAddress;
                    cmd.Parameters.Add("@address", SqlDbType.VarChar).Value = user.Address;
                    cmd.Parameters.Add("@country", SqlDbType.VarChar).Value = user.Country;
                    cmd.Parameters.Add("@state", SqlDbType.VarChar).Value = user.State;
                    cmd.Parameters.Add("@zipcode", SqlDbType.Int).Value = user.ZipCode;
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            connection.Close();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            connection.Close();
                            System.Diagnostics.Debug.WriteLine("Error in stored procedure");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        //TO-DO: Handle execptions
                    }
                }
            }
            return View(user);
        }

        public ActionResult ViewPurchaseHistory() 
        {
            User user = Session[Models.User.UserSessionString] as User;
            if (user != null)
            {
                List<Cart> userCartList = new List<Cart>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("getPurchaseHistoryByUsername", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = user.UserName;
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userCartList.Add(new Cart()
                                {
                                    Name = reader["product_name"].ToString(),
                                    Cost = reader["cost"].GetHashCode() * reader["quantity"].GetHashCode(),
                                    Quantity = reader["quantity"].GetHashCode(),
                                    Category = reader["category"].ToString()
                                });
                            }
                            connection.Close();
                        }
                        return View(userCartList);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write(ex.ToString());
                    }
                }
            }
            else System.Diagnostics.Debug.Write("User null");


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

        public User getUserData(User user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("getUser", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = user.UserName;
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user.Password = reader["password"].ToString();
                            user.Address = reader["address"].ToString();
                            user.EmailAddress = reader["email"].ToString();
                            user.Country = reader["country"].ToString();
                            user.State = reader["state"].ToString();
                            user.ZipCode = reader["zipcode"].GetHashCode();
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }

            }

            return user;
        }
    }
}