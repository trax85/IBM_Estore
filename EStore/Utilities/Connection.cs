using System;
using System.Collections.Generic;
using System.Configuration;
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
}