using EStore.Models;
using System.Collections.Generic;

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
        List<Cart> GetImagesForCart(List<Cart> cartItems);
        List<Product> GetAllProducts();
        List<string> GetProductCategories();
        void DeleteProduct(string productId);
    }

    public interface IProductDataRepositoryV2
    {
        bool OrderProduct(Cart cartItems, string userName);
    }

    public interface IDashboardDataRepository
    {
        AdminDashboard GetDashBoardCardData();
        AdminDashboard GetAdminDashboardTable();
    }

    public interface ITotalSalesDataRepository
    {
        List<TotalSales> GetPurchaseHistory(string userName);

        List<TotalSales> GetAllPurchaseHistory();

        List<DateTime> GetWeekIntervals();
    }
}