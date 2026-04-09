namespace OnlineLearning.DTO
{
    public class SubscriptionDto
    { 
        public int SubscriptionId { get; set; } 
        public string StudentName { get; set; }
        public string CourseName { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 
        public bool IsActive { get; set; }
    }
}
