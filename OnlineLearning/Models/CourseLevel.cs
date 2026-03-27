using static System.Collections.Specialized.BitVector32;

namespace OnlineLearning.Models
{
    public class CourseLevel
    {
        public int LevelId { get; set; }

        public string LevelName { get; set; } // Beginner, Intermediate, Advanced

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; } 
        public List<Course> Courses { get; set; }
    }

}
