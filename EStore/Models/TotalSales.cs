using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EStore.Models
{
    [Table("total_sales")]
    public class TotalSales
    {
        [Key]
        public int Id { get; set; }
        [NotMapped]
        public string ImageSrc { get; set; }
        public string UserName { get; set; }
        public string ProductName { get; set; }
        [NotMapped]
        public string Category { get; set; }
        public int Cost { get; set; }
        public int Quantity { get; set; }
        public string PaymentType { get; set; }

        [Column(TypeName = "Date")]
        public DateTime Timestamp { get; set; }
    }
}