using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class CourseCategories
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name can't exceed 100 characters")]
        public string CategoryName { get; set; }

        public int? ParentCategoryId { get; set; }
        public bool IsParent { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } 
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
