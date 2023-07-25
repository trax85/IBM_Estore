using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EStore.Models
{
    public class User
    {
        [Required(ErrorMessage ="Please enter your name")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Please enter valid username")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Please enter a password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", 
            ErrorMessage = "Password must be between 6 and 20 characters and contain one " +
            "uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage ="Enter type of user")]
        public string Type { get; set; }

        enum userTypes { 
            Customer,
            Admins
        }
    }
}