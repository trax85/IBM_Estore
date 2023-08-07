﻿using EStore.Models;
using System;
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
        bool UpdateUserStatus(string userName);
        int GetActiveUserCount();
    }

    public interface IProductDataRepository
    {
        bool CreateProduct(Product product);
        bool EditProduct(Product product);
        Product GetProduct(string productName);
        List<TotalSales> GetImagesForTotalSales(List<TotalSales> salesItems);
        List<Product> GetAllProducts();
        List<string> GetProductCategories();
        void DeleteProduct(string productId);
        List<TotalSales> MapProductToCategory(List<TotalSales> saleItems);
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

        List<DateTime> GetWeekIntervals(List<TotalSales> sales);
    }
}