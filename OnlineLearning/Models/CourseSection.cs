using static System.Collections.Specialized.BitVector32;

namespace OnlineLearning.Models
{
    public class CourseSection
    {
        public int SectionId { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string SectionTitle { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }         
        public List<CourseVideo> Videos { get; set; }
    }

}
