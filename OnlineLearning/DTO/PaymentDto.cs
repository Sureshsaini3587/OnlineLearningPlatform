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
    public class RazorpayOrderDTO
    {
        public string RazorpayOrderId { get; set; }

        public string RazorpayPaymentId { get; set; }

        public string RazorpaySignature { get; set; }

        public int CourseId { get; set; } 
        public int SubscriptionId { get; set; }
        public int PlanId { get; set; }
    }
    public class CoursePaymentDTO
    { 
        public int StudentId { get; set; }
        public int CourseId { get; set; }  
        public int SubscriptionId { get; set; }   
        public int PlanId { get; set; } 
        
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
    }
    public class PaymentResponseDTO
    {
        public int PlanId { get; set; }
        public int CourseId { get; set; }
        public decimal Amount { get; set; }
    }
}
