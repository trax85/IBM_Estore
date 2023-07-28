using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace EStore.Models
{
    public class Cart
    {
        public static string CartCountSessionString = "CartCount";
        public static string CartSessionString = "Cart";
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Cost { get; set; }
        public int Quantity { get; set; }
    }
}