using EStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EStore.Utilities.DataRepository
{
    public class DummyUserDataRepository : IUserDataRepository
    {
        public bool CreateUser(User user)
        {
            return true;
        }

        public void DeleteUser(string userName)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAllUsers()
        {
            return DummyData.UserModelDataList;
        }

        public User GetUser(string userName)
        {
            if(userName.Equals(DummyData.UserModelData.UserName))
            {
                return DummyData.UserModelData;
            }
            return new User();
        }

        public User UpdateUser(User user)
        {
            if (user.UserName.Equals(DummyData.UserModelData.UserName))
            {
                return user;
            }
            return new User();
        }

        public User VerifyUser(UserLoginCredentials userCred)
        {
            User user = DummyData.UserModelData;
            User failedUser = new User();
            if (user.Password.Equals(userCred.Password) && user.UserName.Equals(userCred.UserName))
                return user;
            
            else return failedUser;
        }
    }

    public class DummyProductDataRepository : IProductDataRepository
    {
        public bool CreateProduct(Product product)
        {
            return true;
        }

        public void DeleteProduct(string productId)
        {
            throw new NotImplementedException();
        }

        public bool EditProduct(Product product)
        {
            if(product.Name.Equals(DummyData.ProductModelData.Name))
            { return true; } else { return false; }
        }

        public List<Product> GetAllProducts()
        {
            return DummyData.ProductModelDataList;
        }

        public List<Cart> GetImagesForCart(List<Cart> cartItems)
        {
            throw new NotImplementedException();
        }

        public Product GetProduct(string productName)
        {
            if (productName.Equals(DummyData.ProductModelData.Name))
                return DummyData.ProductModelData;
            else return new Product();
        }

        public List<string> GetProductCategories()
        {
            return DummyData.CategoriesList;
        }

        public bool OrderProduct(List<Cart> cartItems, string userName)
        {
            if (userName.Equals(DummyData.UserModelData.UserName) && cartItems.Count > 0)
                return true;
            else return false;
        }
    }

    public class DummyProductDataRepositoryV2 : IProductDataRepositoryV2
    {
        public bool OrderProduct(Cart cartItems, string userName)
        {
            throw new NotImplementedException();
        }
    }

    public class DummyDashboardDataRepository : IDashboardDataRepository
    {
        public AdminDashboard GetAdminDashboardTable()
        {
            return DummyData.AdminDashboard;
        }

        public AdminDashboard GetDashBoardCardData()
        {
            return DummyData.AdminDashboard;
        }
    }

    public class DummyCartDataRepository : ICartDataRepository
    {
        public List<Cart> GetPurchaseHistory(string userName)
        {
            return DummyData.CartList;
        }
    }
}