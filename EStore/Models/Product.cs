using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace EStore.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Required(ErrorMessage ="Please enter Product Name")]
        public string Name { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }

        public byte[] ImageData { get; set; }

        [NotMapped]
        public string ImageSrc { get; set; }

        [Required(ErrorMessage = "Please choose product category")]
        public string Category { get; set; }

        public string Description { get; set; }

        public string AdditionalDescription { get; set; }

        [Required(ErrorMessage ="Please enter cost of product")]
        [RegularExpression(@"^[0-9]*$")]
        public int Cost { get; set; }

        public DateTime timestamp { get; set; }
        public List<string> ProductCategories { get; set; }

        public bool IsEmpty()
        {
            if (Name.IsEmpty() || Description.IsEmpty() || AdditionalDescription.IsEmpty() || Category.IsEmpty())
                return true;
            return false;
        }
    }
}