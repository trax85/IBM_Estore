using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace EStore.Models
{
    public class UserLoginCredentials
    {
        [Key]
        [Required(ErrorMessage = "Please enter valid username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$",
            ErrorMessage = "Password must be between 6 and 20 characters and contain one " +
            "uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; }
    }

    [Table("users")]
    public class User : UserLoginCredentials
    {
        public static string UserSessionString = "User";

        [Required(ErrorMessage ="Please enter your name")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [NotMapped]
        [Compare("Password", ErrorMessage ="Passwords doesn't match")]
        public string ComfirmPassword { get; set; }

        public string Type { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Enter valid email address.")]
        public string EmailAddress { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public string  State { get; set; }

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter valid zip code")]
        public int ZipCode { get; set; }

        public DateTime timestamp { get; set; }

        public DateTime Lastseen { get; set; }

        public string SecurityToken { get; set; }
        public DateTime TokenTimeStamp { get; set; }
        public enum UserTypes { 
            Customer,
            Admin
        }

        public bool IsEmpty()
        {
            if(UserName.IsEmpty() || Password.IsEmpty() || FirstName.IsEmpty() 
                || LastName.IsEmpty() || EmailAddress.IsEmpty()) return true;
            return false;
        }
    }
}