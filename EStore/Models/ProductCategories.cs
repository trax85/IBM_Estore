using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EStore.Models
{
    [Table("product_types")]
    public class ProductCategories
    {
        [Key]
        public string Type { get; set; }
    }
}