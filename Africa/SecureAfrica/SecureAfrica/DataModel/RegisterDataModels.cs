using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureAfrica.DataModel
{
    public class RegisterDataModels
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string InternationalPrefix { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        // [Remote(action: "IsEmailInUse", controller: "Account")]
        //    [ValidEmailDomain(allowedDomain: "gmail.com", ErrorMessage = "Email domain must be gmail.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]

        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Address { get; set; }
        public float CoordinateX { get; set; }
        public float CoordinateY { get; set; }
        public string Source { get; set; }
        public string PushTokenId { get; set; }
    }
}
