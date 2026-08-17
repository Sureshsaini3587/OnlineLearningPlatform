using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class SubSection
    {
        [Key]
        public int SubSectionId { get; set; }

        [Required(ErrorMessage = "Sub-section title is required")]
        [StringLength(150)]
        public string SubSectionTitle { get; set; } = string.Empty; 
        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
         
        [Required]
        public int SectionId { get; set; }

        [ForeignKey("SectionId")]
        public virtual CourseSection? CourseSection { get; set; }
    }
}
