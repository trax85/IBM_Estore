using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using EStore.Models;
using System.Web.WebPages;
using System.Collections.Generic;
using EStore.Utilities.DataRepository;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace EStore.Utilities
{
    public class UserDataRepository : SqlDbConnection, IUserDataRepository
    {
        public User VerifyUser(UserLoginCredentials userCred)
        {
            User user = _dbContext.UserModel.FirstOrDefault(u => u.UserName == userCred.UserName && u.Password == userCred.Password);
            if (user != null)
                return user;
            return new User();
        }

        public bool CreateUser(User user)
        {
            User findUser = _dbContext.UserModel.FirstOrDefault(u => u.UserName == user.UserName);
            if(findUser == null)
            {
                try
                {
                    user.timestamp = DateTime.Now.Date;
                    _dbContext.UserModel.Add(user);
                    if (_dbContext.SaveChanges() > 0)
                        return true;
                } catch(DbUpdateException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            
            return false;
        }

        public User GetUser(string userName)
        {
            User user = _dbContext.UserModel.FirstOrDefault(u => u.UserName == userName);
            return user;
        }

        public User UpdateUser(User user)
        {
            var userToUpdate = _dbContext.UserModel.FirstOrDefault(u => u.UserName == user.UserName);
            if(userToUpdate != null)
            {
                try
                {
                    userToUpdate.FirstName = user.FirstName;
                    userToUpdate.LastName = user.LastName;
                    userToUpdate.UserName = user.UserName;
                    userToUpdate.Password = user.Password;
                    userToUpdate.ComfirmPassword = user.ComfirmPassword;
                    userToUpdate.EmailAddress = user.EmailAddress;
                    userToUpdate.Address = user.Address;
                    userToUpdate.Country = user.Country;
                    userToUpdate.State = user.State;
                    userToUpdate.ZipCode = user.ZipCode;
                    _dbContext.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            // Log or handle the validation error details
                            System.Diagnostics.Debug.WriteLine($"Entity: {validationErrors.Entry.Entity.GetType().Name}, Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                        }
                    }
                }
                
                return userToUpdate;
            }
            
            return new User();
        }

        public List<User> GetAllUsers()
        {
            return _dbContext.UserModel.ToList();
        }

        public void DeleteUser(string userName)
        {
            var userToDelete = _dbContext.UserModel.FirstOrDefault(u => u.UserName.Equals(userName));
            if(userToDelete != null)
            {
                try
                {
                    _dbContext.UserModel.Remove(userToDelete);
                    _dbContext.SaveChanges();
                }
                catch(DbUpdateException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }     
    }
}