using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;

namespace OnlineLearning.Models
{
    public class CourseSection
    {
        public int SectionId { get; set; }

        [Required(ErrorMessage ="Course is required")]
        public int CourseId { get; set; }
        [Required(ErrorMessage ="Section Name required")]
        public string SectionTitle { get; set; }  
        public int SortOrder { get; set; } 
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } 
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }  
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }   
    }

}
