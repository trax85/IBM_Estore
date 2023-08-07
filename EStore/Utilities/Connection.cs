using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EStore.Utilities
{
    public class Connection : IDisposable
    {
        protected string _sqlConnectionString;
        protected SqlConnection _connection;

        public Connection()
        {
            _sqlConnectionString = ConfigurationManager.ConnectionStrings["MSSQLServer"].ConnectionString;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }

    public class SqlDbContext : DbContext
    {
        public DbSet<Models.User> UserModel { get; set; }
        public DbSet<Models.Product> ProductModel { get; set; }
        public DbSet<Models.ProductCategories> ProductCategoriesModel { get; set; }
        public DbSet<Models.TotalSales> TotalSalesModel { get; set; }
        public DbSet<Models.ContactUs> ContactUsModel { get; set; }
        public SqlDbContext() : base(ConfigurationManager.ConnectionStrings["MSSQLServer"].ConnectionString) 
        { }
    }

    public class SqlDbConnection : IDisposable
    {
        protected SqlDbContext _dbContext;
        public SqlDbConnection()
        {
            _dbContext = new SqlDbContext();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}