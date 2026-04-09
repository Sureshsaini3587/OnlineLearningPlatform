namespace OnlineLearning.DTO
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }  
        public int SubscriptionId { get; set; }  
        public string StudentName { get; set; }
        public string PlanName { get; set; }
        public string CourseName { get; set; } 
        public decimal Amount { get; set; } 
        public string PaymentStatus { get; set; }  
        public string PaymentMethod { get; set; } 
        public string TransactionId { get; set; } 
        public DateTime PaymentDate { get; set; }
    }
}
