using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using EStore.Models;
using System.Web.WebPages;
using System.Collections.Generic;
using EStore.Utilities.DataRepository;

namespace EStore.Utilities
{
    public class UserDataRepository : Connection, IUserDataRepository
    {
        public User VerifyUser(UserLoginCredentials userCred)
        {
            User user = new User();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("verifyUser", _connection))
            {
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.UserName), SqlDbType.VarChar)
                    .Value = userCred.UserName;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.Password), SqlDbType.VarChar)
                    .Value = userCred.Password;
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

        public bool CreateUser(User user)
        {
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("createUser", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.FirstName), SqlDbType.VarChar).Value = user.FirstName;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.LastName), SqlDbType.VarChar).Value = user.LastName;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.UserName), SqlDbType.VarChar).Value = user.UserName;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.Password), SqlDbType.VarChar).Value = user.Password;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.EmailAddress), SqlDbType.VarChar).Value = user.EmailAddress;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.Address), SqlDbType.VarChar).Value = user.Address;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.Country), SqlDbType.VarChar).Value = user.Country;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.State), SqlDbType.VarChar).Value = user.State;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.ZipCode), SqlDbType.Int).Value = user.ZipCode;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.Type), SqlDbType.VarChar).Value = user.Type;

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

        public User GetUser(string userName)
        {
            User user = new User();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("getUser", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.UserName), SqlDbType.VarChar)
                    .Value = userName;
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

        public User UpdateUser(User user)
        {
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("updateUser", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.FirstName), SqlDbType.VarChar)
                    .Value = user.FirstName;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.LastName), SqlDbType.VarChar)
                    .Value = user.LastName;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.UserName), SqlDbType.VarChar)
                    .Value = user.UserName;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.Password), SqlDbType.VarChar)
                    .Value = user.Password;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.EmailAddress), SqlDbType.VarChar)
                    .Value = user.EmailAddress;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.Address), SqlDbType.VarChar)
                    .Value = user.Address;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.Country), SqlDbType.VarChar)
                    .Value = user.Country;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.State), SqlDbType.VarChar)
                    .Value = user.State;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.ZipCode), SqlDbType.Int)
                    .Value = user.ZipCode;
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

        public List<User> GetAllUsers()
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

        public void DeleteUser(string userName)
        {
            User user = new User();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("deleteUser", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => user.UserName), SqlDbType.VarChar).Value = userName;
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