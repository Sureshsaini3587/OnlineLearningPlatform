using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;

namespace OnlineLearning.Models
{
    public class Course
    {
        public int CourseId { get; set; }
          
        [Required(ErrorMessage = "Course title is required")]
        [StringLength(200, ErrorMessage = "Course title can't exceed 200 characters")]
        public string CourseTitle { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Level is required")]
        public int Level { get; set; }

        [Required(ErrorMessage = "Language is required")] 
        public string Language { get; set; }

        [Required(ErrorMessage = "Instructor is required")]
        public int InstructorId { get; set; }

        [StringLength(500)]
        public string? Thumbnail { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000)]
        public string Description { get; set; }

        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }  
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
       
    } 
     
}
