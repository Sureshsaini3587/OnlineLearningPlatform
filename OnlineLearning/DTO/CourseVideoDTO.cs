namespace OnlineLearning.Models
{

    public class CourseVideoDTO
    {
        public int VideoId { get; set; }

        public int SectionId { get; set; }
        public string SectionName { get; set; } 
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public string VideoUrl { get; set; }
        public int Duration { get; set; } = 0;
        public bool IsDemo { get; set; }
        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
