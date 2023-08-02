using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using EStore.Models;
using System.Reflection;
using EStore.Utilities.DataRepository;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;

namespace EStore.Utilities
{
    public class ModelReader
    {
        public User getUser(SqlDataReader reader)
        {   User user = new User();
            if (reader.HasRows)
            {
                reader.Read();
                user = copyToUser(reader);
            }
            return user;
        }

        public List<User> getUserList(SqlDataReader reader)
        {
            List<User> users = new List<User>();
            while(reader.Read())
            {
                User user = copyToUser(reader);
                users.Add(user);
            }
            return users;
        }

        private User copyToUser(SqlDataReader reader)
        {
            User user = new User();
            user.FirstName = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => user.UserName));
            user.LastName = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => user.LastName));
            user.UserName = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => user.UserName));
            user.Password = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => user.Password));
            user.Address = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => user.Address));
            user.EmailAddress = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => user.EmailAddress));
            user.Country = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => user.Country));
            user.State = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => user.State));
            user.ZipCode = ReadInt32Column(reader, VariableNameHelper.GetPropertyName(() => user.ZipCode));
            user.Type = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => user.Type));
            return user;
        }

        public Product getProduct(SqlDataReader reader)
        {
            Product product = new Product();
            if (reader.HasRows)
            {
                reader.Read();
                product = copyToProduct(reader);
            }
            return product;
        }

        public List<Product> getProductList(SqlDataReader reader)
        {
            List<Product> productList = new List<Product>();
            while (reader.Read())
            {
                Product product = copyToProduct(reader);
                productList.Add(product);
            }

            return productList;
        }

        private Product copyToProduct(SqlDataReader reader)
        {
            Product product = new Product();
            product.Name = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => product.Name));
            product.Category = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => product.Category));
            product.Description = ReadStringColumn(reader, 
                VariableNameHelper.GetPropertyName(() => product.Description));
            product.AdditionalDescription = ReadStringColumn(reader, 
                VariableNameHelper.GetPropertyName(() => product.AdditionalDescription));
            product.Cost = ReadInt32Column(reader, VariableNameHelper.GetPropertyName(() => product.Cost));

            return product;
        }

        public List<string> GetProductCategories(SqlDataReader reader)
        {
            List<string> products = new List<string>();
            while (reader.Read()) 
            {
                products.Add(ReadStringColumn(reader, "ProductCategories"));
            }

            return products;
        }

        public List<Cart> getCartList(SqlDataReader reader)
        {
            List<Cart> cartList = new List<Cart>();
            while (reader.Read())
            {
                Cart cart = new Cart();
                cart.Name = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => cart.Name));
                cart.Category = ReadStringColumn(reader, VariableNameHelper.GetPropertyName(() => cart.Category));
                cart.Quantity = ReadInt32Column(reader, VariableNameHelper.GetPropertyName(() => cart.Quantity));
                cart.Cost = ReadInt32Column(reader, VariableNameHelper.GetPropertyName(() => cart.Cost));
                cartList.Add(cart);
            }

            return cartList;
        }

        public AdminDashboard getDashboardCard(SqlDataReader reader)
        {
            AdminDashboard dashboard = new AdminDashboard();
            if (reader.HasRows)
            {
                reader.Read();
                dashboard.GrossSalesAmount = ReadInt32Column(reader, 
                    VariableNameHelper.GetPropertyName(() => dashboard.GrossSalesAmount));
                dashboard.TotalProductsSold = ReadInt32Column(reader, 
                    VariableNameHelper.GetPropertyName(() => dashboard.TotalProductsSold));
                dashboard.TotalUsers = ReadInt32Column(reader,
                    VariableNameHelper.GetPropertyName(() => dashboard.TotalUsers));

                dashboard.WSalesAmount = ReadInt32Column(reader, "TotalSalesAmountThisWeek");
                int pwSalesAmount = ReadInt32Column(reader, "TotalSalesAmountPreviousWeek");
                dashboard.WVSalesAmount =
                    (float)Math.Round(((float)(dashboard.WSalesAmount - pwSalesAmount) / (float)pwSalesAmount) * 100, 2);

                dashboard.WProductsSold = ReadInt32Column(reader, "TotalProductsSoldThisWeek");
                int pwProductsSold = ReadInt32Column(reader, "TotalProductsSoldPreviousWeek");
                System.Diagnostics.Debug.WriteLine("Sold:" + pwProductsSold);
                dashboard.WVProductsSold =
                    (float)Math.Round(((float)(dashboard.WProductsSold - pwProductsSold) / (float)pwProductsSold) * 100, 2);

                dashboard.WProductsAddition = ReadInt32Column(reader, "TotalProductsAddedThisWeek");
                int pwTotalProducts = ReadInt32Column(reader, "TotalProductsAddedPreviousWeek");
                
                dashboard.WVProductsAddition =
                    (float)Math.Round(((float)(dashboard.WProductsAddition - pwTotalProducts) / (float)pwTotalProducts) * 100, 2);

                dashboard.WUsersAddition = ReadInt32Column(reader, "TotalUsersThisWeek");
                int pwTotalUsers = ReadInt32Column(reader, "TotalUsersPreviousWeek");
                dashboard.WVUsersAddition =
                    (float)Math.Round(((float)(dashboard.WUsersAddition - pwTotalUsers) / (float)pwTotalUsers) * 100, 2);
            }
            return dashboard;
        }

        public AdminDashboard GetDashboardTable(SqlDataReader reader)
        {
            AdminDashboard dashboard = new AdminDashboard();
            if (reader.HasRows)
            {
                dashboard.ProductCategories = new List<string>();
                dashboard.CategoryCount = new List<int>();
                dashboard.TotalCost = new List<int>();
                while (reader.Read())
                {
                    dashboard.ProductCategories.Add(ReadStringColumn(reader, 
                        VariableNameHelper.GetPropertyName(() => dashboard.ProductCategories)));
                    dashboard.CategoryCount.Add(ReadInt32Column(reader,
                        VariableNameHelper.GetPropertyName(() => dashboard.CategoryCount)));
                    dashboard.TotalCost.Add(ReadInt32Column(reader,
                        VariableNameHelper.GetPropertyName(() => dashboard.TotalCost)));
                }
            }

            return dashboard;
        }

        public int ReadInt32Column(SqlDataReader reader, string columnName)
        {
            try
            {
                return reader[columnName].GetHashCode();
            }
            catch (Exception) { }
            return 0;
        }

        public string ReadStringColumn(SqlDataReader reader, string columnName)
        {
            try
            {
                return reader[columnName].ToString();
            }
            catch (Exception) {
                System.Diagnostics.Debug.WriteLine("colname:" + columnName);
            }
            return "";
        }
    }
}