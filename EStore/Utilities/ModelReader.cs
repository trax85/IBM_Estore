using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using EStore.Models;
using EStore.Utilities.DataRepository;

namespace EStore.Utilities
{
    public class ModelReader
    {
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
                dashboard.WVSalesAmount = GetVariance(dashboard.WSalesAmount, pwSalesAmount);

                dashboard.WProductsSold = ReadInt32Column(reader, "TotalProductsSoldThisWeek");
                int pwProductsSold = ReadInt32Column(reader, "TotalProductsSoldPreviousWeek");
                System.Diagnostics.Debug.WriteLine("Sold:" + pwProductsSold);
                dashboard.WVProductsSold = GetVariance(dashboard.WProductsSold, pwProductsSold);

                dashboard.WProductsAddition = ReadInt32Column(reader, "TotalProductsAddedThisWeek");
                int pwTotalProducts = ReadInt32Column(reader, "TotalProductsAddedPreviousWeek");
                dashboard.WVProductsAddition = GetVariance(dashboard.WProductsAddition, pwTotalProducts);

                dashboard.WUsersAddition = ReadInt32Column(reader, "TotalUsersThisWeek");
                int pwTotalUsers = ReadInt32Column(reader, "TotalUsersPreviousWeek");
                dashboard.WVUsersAddition = GetVariance(dashboard.WUsersAddition, pwTotalUsers);
            }
            return dashboard;
        }

        private float GetVariance(float currentWeek, float previousWeek) 
        {
            if(currentWeek == 0 && previousWeek == 0)
            {
                return 0;
            }
            else if(currentWeek <= 0)
            {
                return (float)Math.Round(((1) / previousWeek) * 100, 2);
            } else if (previousWeek <= 0)
            {
                return (float)Math.Round(((currentWeek - previousWeek) / 1) * 100, 2);
            }
            return (float)Math.Round(((currentWeek - previousWeek) / previousWeek) * 100, 2);
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