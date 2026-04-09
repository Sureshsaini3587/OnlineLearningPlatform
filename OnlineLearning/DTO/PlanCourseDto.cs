namespace OnlineLearning.DTO
{
    public class PlanCourseDto
    {
        public int PlanCourseId { get; set; }
        public int PlanId { get; set; }
        public int CourseId { get; set; }
    }
    public class PlanCourseListDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
    }
}
