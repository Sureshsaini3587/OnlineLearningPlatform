using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class Instructor
    {
        public int? InstructorId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "MobileNo is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        [DataType(DataType.PhoneNumber)]
        public string? Mobile { get; set; } 
        public string? Role { get; set; }  
        public string? Bio { get; set; }
        public int? ExperienceYears { get; set; }
        public byte[]? ProfileImage { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
}
