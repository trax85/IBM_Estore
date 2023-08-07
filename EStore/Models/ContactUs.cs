using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EStore.Models
{
    public class ContactUs
    {
        [Key]
        public string CompanyName { get; set; }
        [RegularExpression(@"^www\..*",
            ErrorMessage = "Enter valid Url address.")]
        public string CompanyUrl { get; set; }
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "Enter valid Phone Number.")]
        public string Phone { get; set; }
        [RegularExpression(@"^www\..*",
            ErrorMessage = "Enter valid Url address.")]
        public string WebsiteUrl { get; set; }
        public string ProgramDetails { get; set; }
        public string Address { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Enter valid mail address.")]
        public string SupportMail { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Enter valid mail address.")]
        public string MarketingMail { get; set; }
    }
}