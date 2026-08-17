namespace OnlineLearning.Models
{
    public class StudentReadingVM
    {
        public List<EnrolledCourseVM> EnrolledCourses { get; set; } = new List<EnrolledCourseVM>();
    }

    public class EnrolledCourseVM
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string LevelName { get; set; }
        public string CategoryName { get; set; }
        public DateTime EndDate { get; set; } 
        public List<CourseSectionVM> Sections { get; set; } = new List<CourseSectionVM>();
    }

    public class CourseSectionVM
    {
        public int SectionId { get; set; }
        public string SectionTitle { get; set; }
    }
}
