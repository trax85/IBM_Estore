using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EStore.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace EStore.Controllers
{
    public class AdminController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MSSQLServer"].ConnectionString;
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
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand("getDashboardCardData", connection))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            connection.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    dashboard.GrossSalesAmount = reader["GrossSales"].GetHashCode();
                                    dashboard.TotalProductSold = reader["TotalSold"].GetHashCode();
                                    dashboard.TotalUsers = reader["TotalUsers"].GetHashCode();

                                    dashboard.WSalesAmount = reader["CWTotalSales"].GetHashCode();
                                    int pwSalesAmount = reader["PWTotalSales"].GetHashCode();
                                    dashboard.WVSalesAmount =
                                        (float)Math.Round(((float)(dashboard.WSalesAmount - pwSalesAmount) / (float)pwSalesAmount) * 100, 2);

                                    dashboard.WProductsAddition = reader["CWTotalProducts"].GetHashCode();
                                    int pwTotalProducts = reader["PWTotalProducts"].GetHashCode();
                                    dashboard.WVProductsAddition =
                                        (float)Math.Round(((float)(dashboard.WProductsAddition - pwTotalProducts) / (float)pwTotalProducts) * 100, 2);


                                    dashboard.WUsersAddition = reader["CWTotalUsers"].GetHashCode();
                                    int pwTotalUsers = reader["PWTotalUsers"].GetHashCode();
                                    dashboard.WVUsersAddition =
                                        (float)Math.Round(((float)(dashboard.WUsersAddition - pwTotalUsers) / (float)pwTotalUsers) * 100, 2);
                                }
                                else System.Diagnostics.Debug.WriteLine("No data");
                            }
                            connection.Close();
                            if (HttpContext.Application["UserCount"] != null)
                                dashboard.TotalLoggedUsers = (int)HttpContext.Application["UserCount"];
                            else dashboard.TotalLoggedUsers = 0;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }
                    }
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand("getDashboardTable", connection))
                    {
                        try
                        {
                            dashboard.ProductCategories = new List<string>();
                            dashboard.CategoryCount = new List<int>();
                            dashboard.TotalCost = new List<int>();
                            cmd.CommandType = CommandType.StoredProcedure;
                            connection.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    dashboard.ProductCategories.Add(reader["ProductCategory"].ToString());
                                    dashboard.CategoryCount.Add(reader["CategoryCount"].GetHashCode());
                                    dashboard.TotalCost.Add(reader["TotalCost"].GetHashCode());
                                }
                            }
                        } 
                        catch (Exception ex) 
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }
                    }
                    return View(dashboard);
                }
            }
            return RedirectToAction("SignIn", "Login");
        }

        public ActionResult Users()
        {
            List<User> userList = new List<User>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("getAllUsers", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                User user = new User()
                                {
                                    FirstName = reader["firstname"].ToString(),
                                    LastName = reader["lastname"].ToString(),
                                    UserName = reader["username"].ToString(),
                                    State = reader["state"].ToString(),
                                    Country = reader["country"].ToString(),
                                    EmailAddress = reader["email"].ToString(),
                                    Type = reader["type"].ToString()
                                };
                                userList.Add(user);
                            }
                        }
                    }
                    connection.Close();
                }
                catch(Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
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
            System.Diagnostics.Debug.WriteLine("user:" + userid);
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("getUser", connection))
            {
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = userid;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            User user = new User()
                            {
                                FirstName = reader["firstname"].ToString(),
                                LastName = reader["lastname"].ToString(),
                                UserName = reader["username"].ToString(),
                                Password = reader["password"].ToString(),
                                EmailAddress = reader["email"].ToString(),
                                State = reader["state"].ToString(),
                                Country = reader["country"].ToString(),
                                ZipCode = reader["zipcode"].GetHashCode(),
                                Type = reader["type"].ToString(),
                            };
                            connection.Close();
                            return View("CreateEditUser", user);
                        } 
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public ActionResult Save(User user, bool isEdit)
        {
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("Model Valid");
                string query;
                if (isEdit)
                {
                    query = "updateUser";
                }
                else query = "createUser";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, connection))
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
                    if (!isEdit) cmd.Parameters.Add("@type", SqlDbType.VarChar).Value = user.Type;
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            System.Diagnostics.Debug.WriteLine("Redirect");
                            connection.Close();
                            return RedirectToAction("Users");
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
            if(isEdit) TempData["isEdit"] = "true";
            else TempData["isEdit"] = "false";

            return View("CreateEditUser", user);
        }

        public ActionResult DeleteUser(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("deleteUser", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = userId;
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

            return RedirectToAction("Users");
        }

        public ActionResult Products()
        {
            List<Product> productList = new List<Product>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("getAllProducts", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Product product = new Product()
                                {
                                    Name = reader["name"].ToString(),
                                    Category = reader["category"].ToString(),
                                    Description = reader["description"].ToString(),
                                    Cost = reader["cost"].GetHashCode(),
                                    AdditionalDescription = reader["additional_description"].ToString()
                                };
                                productList.Add(product);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

            return View(productList);
        }

        public ActionResult CreateProduct()
        {
            TempData["isEdit"] = "false";
            Product product = new Product()
            {
                products = new List<string>()
                {
                    "Stationery",
                    "Electronics",
                    "Grocessory"
                }
            };
            return View("CreateEditProduct", product);
        }

        public ActionResult EditProduct(string productId)
        {
            TempData["isEdit"] = "true";
            System.Diagnostics.Debug.WriteLine(productId + "name");
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("getProductByName", connection))
            {
                cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = productId;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            Product product = new Product()
                            {
                                Name = reader["name"].ToString(),
                                Category = reader["category"].ToString(),
                                Description = reader["description"].ToString(),
                                AdditionalDescription = reader["additional_description"].ToString(),
                                Cost = reader["cost"].GetHashCode(),
                                products = new List<string>()
                                {
                                    "Stationery",
                                    "Electronics",
                                    "Grocessory"
                                }
                            };
                            connection.Close();
                            System.Diagnostics.Debug.WriteLine("cat:" + product.Category);
                            return View("CreateEditProduct", product);
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return RedirectToAction("Products");
        }

        public ActionResult DeleteProduct(string productId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("deleteProduct", connection))
            {
                cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = productId;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return RedirectToAction("Products");
        }

        [HttpPost]
        public ActionResult SaveProduct(Product product, bool isEdit)
        {
            if (ModelState.IsValid)
            {
                string query;
                if (isEdit) query = "updateProduct";
                else query = "createProduct";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = product.Name;
                    cmd.Parameters.Add("@category", SqlDbType.VarChar).Value = product.Category;
                    cmd.Parameters.Add("@description", SqlDbType.VarChar).Value = product.Description;
                    cmd.Parameters.Add("@AdditionalDescription", SqlDbType.VarChar).Value = product.AdditionalDescription;
                    cmd.Parameters.Add("@cost", SqlDbType.Int).Value = product.Cost;
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                        return RedirectToAction("Products");
                    } catch(Exception ex)
                    {
                        TempData["ErrorMessage"] = "Failed to Save Product, Please Try again.";
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
             }
            if(isEdit) TempData["isEdit"] = "true";
            else TempData["isEdit"] = "false";
            return View("CreateEditProduct",product);
        }
    }
}