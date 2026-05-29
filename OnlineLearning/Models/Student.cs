

using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class Student
    {
        public int? StudentId { get; set; }
        public int? UserID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage ="Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage ="MobileNo is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        [DataType(DataType.PhoneNumber)]
        public string Mobile { get; set; } 
        public string? Role { get; set; } 
        public DateTime? DOB { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public byte[]? ProfileImage { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
    public class StudentProfileViewModel
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
         
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid mobile number")]
        public string Mobile { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; } 
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public byte[]? ProfileImage { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
