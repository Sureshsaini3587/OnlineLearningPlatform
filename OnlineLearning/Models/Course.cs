using static System.Collections.Specialized.BitVector32;

namespace OnlineLearning.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int CategoryName { get; set; }
        public int InstructorId { get; set; }
        public decimal Price { get; set; }
        public string Thumbnail { get; set; }
        public string Language { get; set; }
        public int LevelId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Coursecategories Category { get; set; }
        public CourseLevel Level { get; set; }
        public List<CourseSection> Sections { get; set; }
    }
    
}
