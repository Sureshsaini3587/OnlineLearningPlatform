using static System.Collections.Specialized.BitVector32;

namespace OnlineLearning.Models
{

    public class CourseVideo
    {
        public int VideoId { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; }

        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; } = 0;
        public bool IsDemo { get; set; }
        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
