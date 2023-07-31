using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EStore.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace EStore.Controllers
{
    public class LoginController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MSSQLServer"].ConnectionString;
        public ActionResult SignIn() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(UserLoginCredentials userCred)
                        user.FirstName = reader["firstname"].ToString();
                        user.LastName = reader["lastname"].ToString();
                        user.UserName = userCred.UserName;

                            connection.Close();
                            HttpContext.Session[Models.User.UserSessionString] = user;
                            if (user.Type == Models.User.UserTypes.Customer.ToString())
                                return RedirectToAction("Index", "Home");
                            else //This needs to be re-routed 
                                return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            connection.Close();
                            TempData["ErrorMessage"] = "Please enter correct username/password";
                        System.Diagnostics.Debug.WriteLine("Please enter correct username/password");
                    }
                    catch (Exception ex)
                    {
                        //TO-DO: Handle execptions
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("createUser", connection))
                {
                    try
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
                        cmd.Parameters.Add("@type", SqlDbType.VarChar).Value = Models.User.UserTypes.Customer.ToString();
                        
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        
                        if (reader.HasRows)
                        {
                            connection.Close();
                            return RedirectToAction("SignIn", "Login");
                        }
                        else
                        {
                            connection.Close();
                            System.Diagnostics.Debug.WriteLine("Error in stored procedure");
                            TempData["ErrorMessage"] = "Username already exists choose a different username";
                            return View(user);
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
    }
}