using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EStore.Models
{
    public class Product
    {
        [Required(ErrorMessage ="Please enter Product Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please choose product category")]
        public string Category { get; set; }

        public string Description { get; set; }

        public string AdditionalDescription { get; set; }

        [Required(ErrorMessage ="Please enter cost of product")]
        [RegularExpression(@"^[0-9]*$")]
        public int Cost { get; set; }

        public List<string> products { get; set; }
        public enum ProductTypes
        {
            Electronics,
            Grocessory,
            Stationery
        }
    }
}