using EStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EStore.Utilities
{
    public class DummyData
    {
        public static User UserModelData = new User()
        {
            FirstName = "test firstname",
            LastName = "test lastname",
            UserName = "testUsername",
            Password = "test@123",
            EmailAddress = "test@mail.com",
            Country = "testCountry",
            State = "testState",
            ZipCode = 000001,
            Type = "Customer"
        };

        public static User AdminUserModelData = new User()
        {
            FirstName = "test firstname",
            LastName = "test lastname",
            UserName = "testAdmin",
            Password = "test@123",
            EmailAddress = "test@mail.com",
            Country = "testCountry",
            State = "testState",
            ZipCode = 000001,
            Type = "Admin"
        };

        public static List<User> UserModelDataList = new List<User> { 
            UserModelData,
            UserModelData,
            UserModelData
        };

        public static Product ProductModelData = new Product()
        {
            Name = "ProductItem",
            Description = "Description",
            AdditionalDescription = "Additional Description",
            Category = "Category 1",
            Cost = 3400
        };

        public static List<Product> ProductModelDataList = new List<Product>
        {
            ProductModelData,
            ProductModelData, 
            ProductModelData
        };

        public static List<string> CategoriesList = new List<string>
        {
            "Category 1",
            "Category 2",
            "Category 3",
        };

        public static AdminDashboard AdminDashboard = new AdminDashboard()
        {
            GrossSalesAmount = 1000,
            TotalLoggedUsers = 2,
            TotalProductsSold = 4,
            TotalUsers = 3,
            WSalesAmount = 300,
            WVSalesAmount = 10,
            WProductsSold = 2,
            WVProductsSold = 3,
            WProductsAddition = 4,
            WVProductsAddition = 2,
            WUsersAddition = 3,
            WVUsersAddition = 4,
            ProductCategories = CategoriesList,
            
        };

        public static Cart CartData = new Cart()
        {
            Name = "Product 1",
            Category = CategoriesList[1],
            Cost = 300,
            Quantity = 3
        };

        public static List<Cart> CartList = new List<Cart>
        {
            CartData,
            CartData,
            CartData
        };

        public static TotalSales TotalSalesData = new TotalSales()
        {
            Id = 1,
            ProductName = "TestProduct",
            Quantity = 2,
            Cost = 200,
            Category = CategoriesList[1],
            Timestamp = DateTime.Now,
        };

        public static List<TotalSales> TotalSalesList = new List<TotalSales>()
        {
            TotalSalesData,
            TotalSalesData,
            TotalSalesData,
            TotalSalesData
        };

        public static ContactUs ContactUsModel = new ContactUs()
        {
            CompanyName = "TestComp",
            CompanyUrl = "www.testcomp.com",
            Phone = "123456789",
            WebsiteUrl = "www.test.com",
            ProgramDetails = "Test-Test",
            Address = "Address",
            SupportMail = "Support@gmail.com",
            MarketingMail = "Marketing@mail.com"
        };
    }
}