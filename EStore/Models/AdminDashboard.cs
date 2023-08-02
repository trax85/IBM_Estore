using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EStore.Models
{
    public class AdminDashboard
    {
        public int GrossSalesAmount { get; set; }

        public int TotalLoggedUsers { get; set; }

        public int TotalProductsSold { get; set; }

        public int TotalUsers { get; set; }

        // These parameters store weekly data and have coresponding variation data
        public int WSalesAmount { get; set; }

        public float WVSalesAmount { get; set; }

        public float WProductsSold { get; set; }

        public float WVProductsSold { get; set; }

        public int WProductsAddition { get; set; }

        public float WVProductsAddition { get; set; }

        public int WUsersAddition { get; set; }

        public float WVUsersAddition { get; set; }

        // These parameters are for table card

        public List<string> ProductCategories { get; set; }
        public List<int> CategoryCount { get; set; }
        public List<int> TotalCost { get; set; }
    }
}