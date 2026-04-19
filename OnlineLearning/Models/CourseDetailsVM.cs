namespace OnlineLearning.Models
{
    public class CourseDetailsVM
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Thumbnail { get; set; }

        public string LevelName { get; set; }
        public string CategoryName { get; set; }

        public int TotalLectures { get; set; }
        public bool HasDemo { get; set; }

        public List<SectionVM> Sections { get; set; }
    }
    public class SectionVM
    {
        public string SectionTitle { get; set; }
        public List<VideoVM> Videos { get; set; }
    }

    public class VideoVM
    {
        public int VideoId { get; set; }
        public string Title { get; set; }
        public bool IsDemo { get; set; }
        public string VideoUrl { get; set; } 
    }
}
