using EStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EStore.Utilities.DataRepository
{
    public interface IUserDataRepository
    {
        User VerifyUser(UserLoginCredentials userCred);
        bool CreateUser(User user);
        User GetUser(string userName);
        User UpdateUser(User user);
        List<User> GetAllUsers();
        void DeleteUser(string userName);
    }

    public interface IProductDataRepository
    {
        bool CreateProduct(Product product);
        bool EditProduct(Product product);
        Product GetProduct(string productName);
        List<Product> GetAllProducts();
        bool OrderProduct(List<Cart> cartItems, string userName);
        List<string> GetProductCategories();
        void DeleteProduct(string productId);
    }

    public interface IDashboardDataRepository
    {
        AdminDashboard GetDashBoardCardData();
        AdminDashboard GetAdminDashboardTable();
    }

    public interface ICartDataRepository
    {
        List<Cart> GetPurchaseHistory(string userName);

    }
}