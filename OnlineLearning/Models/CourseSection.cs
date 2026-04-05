using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;

namespace OnlineLearning.Models
{
    public class CourseSection
    {
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Course is required")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Section name is required")]
        [StringLength(200, ErrorMessage = "Section title can't exceed 200 characters")]
        public string SectionTitle { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sort order must be greater than 0")]
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } 
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }  
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }   
    }

}
