using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using EStore.Models;
using System.Web.WebPages;
using System.Collections.Generic;

namespace EStore.Utilities
{
    public class UserDataRepository : Connection
    {
        public User VerifyUser(UserLoginCredentials userCred)
        {
            User user = new User();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("verifyUser", _connection))
            {
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = userCred.UserName;
                cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = userCred.Password;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {                    
                    _connection.Open();
                    ModelReader userModelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        user = userModelReader.getUser(reader);
                    }
                    _connection.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

            return user;
        }

        public bool createUser(User user)
        {
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("createUser", _connection))
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
                cmd.Parameters.Add("@type", SqlDbType.VarChar).Value = user.Type;

                try
                {
                    _connection.Open();
                    ModelReader userModelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        user = userModelReader.getUser(reader);
                        if (!user.UserName.IsEmpty()) return true;
                    }
                    _connection.Close();
                } 
                catch(Exception ex) 
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            return false;
        }

        public User getUser(string userName)
        {
            User user = new User();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("getUser", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = userName;
                try
                {
                    _connection.Open();
                    ModelReader modelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        user = modelReader.getUser(reader);
                    }
                } catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            return user;
        }

        public User updateUser(string userName)
        {
            User user = new User();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("updateUser", _connection))
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
                    _connection.Open();
                    ModelReader modelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        user = modelReader.getUser(reader);
                    }
                } 
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            return user;
        }

        public List<User> getAllUsers()
        { 
            List<User> users = new List<User>();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("getAllUsers", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    _connection.Open();
                    ModelReader modelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        users = modelReader.getUserList(reader);
                    }
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return users;
        }

        public void deleteUser(string userName)
        {
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("deleteUser", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = userName;
                try
                {
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }     
    }
}