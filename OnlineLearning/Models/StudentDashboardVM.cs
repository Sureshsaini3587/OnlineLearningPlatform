namespace OnlineLearning.Models
{
    public class StudentDashboardVM
    {
        public string UserName { get; set; }

        public string PlanName { get; set; }
        public int? DaysLeft { get; set; }

        public int? TotalCourses { get; set; }
        public int? InProgressCourses { get; set; }

        public int? HoursWatched { get; set; }

        public int? PQJScore { get; set; }

        public ContinueCourseVM ContinueCourse { get; set; }

        public List<CourseDTO> RecentCourses { get; set; }

    }
    public class ContinueCourseVM
    {
        public int? CourseId { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }

        public int? Progress { get; set; }

        public int? LastLessonId { get; set; }
    }
}
