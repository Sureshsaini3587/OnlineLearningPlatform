using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
       
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
         
        public bool RememberMe { get; set; }
    }
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [Display(Name = "Mobile Number")] 
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be exactly 10 digits and numeric only.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } 
    }
}
