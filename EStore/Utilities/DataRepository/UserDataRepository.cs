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
            try
            {
                User user = _dbContext.UserModel.FirstOrDefault(u => u.UserName == userCred.UserName && u.Password == userCred.Password);
                if (user != null)
                    return user;
            } 
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            
            return new User();
        }

        public bool CreateUser(User user)
        {

            try
            {
                User findUser = _dbContext.UserModel.FirstOrDefault(u => u.UserName == user.UserName);
                if (findUser == null)
                {
                    try
                    {
                        user.timestamp = DateTime.Now;
                        user.Lastseen = DateTime.Now;
                        // Prevent any chances of getting in
                        user.TokenTimeStamp = DateTime.Now.AddMinutes(-10);
                        _dbContext.UserModel.Add(user);
                        if (_dbContext.SaveChanges() > 0)
                            return true;
                    }
                    catch (DbUpdateException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
            } 
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            
            return false;
        }

        public User GetUser(string userName)
        {
            try
            {
                User user = _dbContext.UserModel.FirstOrDefault(u => u.UserName == userName);
                return user;
            } 
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return new User();
        }

        public User UpdateUser(User user)
        {
            try
            {
                var userToUpdate = _dbContext.UserModel.FirstOrDefault(u => u.UserName == user.UserName);
                if (userToUpdate != null)
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
                        userToUpdate.Lastseen = DateTime.Now;
                        userToUpdate.Type = user.Type;
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
            } 
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            
            return user;
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return _dbContext.UserModel.ToList();
            } 
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return new List<User>();            
        }

        public void DeleteUser(string userName)
        {
            try
            {
                var userToDelete = _dbContext.UserModel.FirstOrDefault(u => u.UserName.Equals(userName));
                if (userToDelete != null)
                {
                    try
                    {
                        _dbContext.UserModel.Remove(userToDelete);
                        _dbContext.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
            } 
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }   
        
        public bool UpdateUserStatus(string userName)
        {
            try
            {
                var userToUpdate = _dbContext.UserModel.FirstOrDefault(u => u.UserName.Equals(userName));
                if (userToUpdate != null)
                {
                    try
                    {
                        userToUpdate.ComfirmPassword = userToUpdate.Password;
                        userToUpdate.Lastseen = DateTime.Now;
                        _dbContext.SaveChanges();
                        return true;
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
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return false;
        }

        public int GetActiveUserCount()
        {
            DateTime fifteenMinutesAgo = DateTime.Now.AddMinutes(-15);

            return _dbContext.UserModel.Where(u => u.Lastseen >= fifteenMinutesAgo).ToList().Count;
        }

        public bool VerifyUser(string emailAddress) 
        {
            try
            {
                User user = _dbContext.UserModel.FirstOrDefault(u => u.EmailAddress.Equals(emailAddress));
                if(user != null)
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return false;
        }

        public bool UpdateUserToken(string token, string emailAddress)
        {
            try
            {
                if(!token.IsEmpty())
                {
                    User user = _dbContext.UserModel.FirstOrDefault(u => u.EmailAddress.Equals(emailAddress));
                    if(user != null)
                    {
                        user.SecurityToken = token;
                        user.TokenTimeStamp = DateTime.Now;
                        user.ComfirmPassword = user.Password;
                        try
                        {
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
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return false;
        }

        public User VerifyUserToken(string token, string emailAddress)
        {
            try
            {
                User user = _dbContext.UserModel.FirstOrDefault(
                    u => u.EmailAddress.Equals(emailAddress) && u.SecurityToken.Equals(token));
                if (user != null)
                {
                    DateTime fiveMinuitsAgo = DateTime.Now.AddMinutes(-5);
                    if (user.TokenTimeStamp > fiveMinuitsAgo)
                    {
                        return user;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return new User();
        }
    }
}