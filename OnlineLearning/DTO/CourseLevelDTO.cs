using static System.Collections.Specialized.BitVector32;

namespace OnlineLearning.Models
{
    public class CourseLevelDTO
    {
        public int LevelId { get; set; }

        public string LevelName { get; set; } // Beginner, Intermediate, Advanced

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } 
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; } 
        public List<Course> Courses { get; set; }
    }

}
