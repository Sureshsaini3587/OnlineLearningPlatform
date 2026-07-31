namespace OnlineLearning.Models
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int ActiveCourses { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int PendingQueries { get; set; }
        public List<EnrollmentDto> RecentEnrollments { get; set; }
        public List<decimal> RevenueData { get; set; }  
    }
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string PlanName { get; set; }
        public string CourseName { get; set; }
        public DateTime EnrollmentDate { get; set; } 
        public string FormattedDate => EnrollmentDate.ToString("MMM dd, yyyy"); 
        public string StudentProfileImageUrl { get; set; }
    }
}
