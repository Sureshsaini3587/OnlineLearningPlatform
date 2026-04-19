namespace OnlineLearning.Models
{
    public class CourseCategoriesDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public string ParentCategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? CourseCount { get; set; }
        public bool IsParent { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
