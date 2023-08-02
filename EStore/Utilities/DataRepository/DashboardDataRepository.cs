using EStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EStore.Utilities.DataRepository
{
    public class DashboardDataRepository : Connection, IDashboardDataRepository
    {
        public AdminDashboard GetDashBoardCardData()
        {
            AdminDashboard dashboard = new AdminDashboard();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("getDashboardCardData", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    _connection.Open();
                    ModelReader modelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dashboard = modelReader.getDashboardCard(reader);
                    }
                    _connection.Close();
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    _connection.Close();
                }
            }

            return dashboard;
        }

        public AdminDashboard GetAdminDashboardTable()
        {
            AdminDashboard dashboard = new AdminDashboard();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("getDashboardTable", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {                    
                    _connection.Open();
                    ModelReader modelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dashboard = modelReader.GetDashboardTable(reader);
                    }
                    _connection.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    _connection.Close();
                }
            }

            return dashboard;
        }
    }
}