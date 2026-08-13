namespace OnlineLearning.Models
{
    public class CourseDetailsVM
    {
        public int CourseId { get; set; }
        public string Id { get; set; }
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
        public int SectionId { get; set; }
        public int SectionOrder { get; set; }
        public string SectionTitle { get; set; }
        public List<VideoVM> Videos { get; set; }
    }

    public class VideoVM
    {
        public int VideoId { get; set; }
        public string SectionTitle { get; set; }
        public string Title { get; set; }
        public bool IsDemo { get; set; }
        public string VideoUrl { get; set; } 
        public int Duration { get; set; } 
        public int VideoOrder { get; set; } 
    }
    public class TestSubmissionVM
    {
        public int CourseId { get; set; }
        public int SectionId { get; set; } 
        public Dictionary<int, int> SelectedAnswers { get; set; } = new Dictionary<int, int>();
    }

    public class TestResultVM
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public double ScorePercentage { get; set; }
    }
}
