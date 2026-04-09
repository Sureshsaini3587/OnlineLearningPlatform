using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class Payment
    {
        public int? PaymentId { get; set; } 
        public int StudentId { get; set; }
        public int CourseId { get; set; } 
        public int SubscriptionPlan { get; set; } 
        public decimal Amount { get; set; } 
        public string Status { get; set; } = "Pending"; // Success, Failed 
        public string PaymentGateway { get; set; } // UPI, Card, etc. 
        public string? TransactionId { get; set; } 
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;  
        public int? CreatedBy { get; set; } 
        public int? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true ; 
        public bool IsDeleted { get; set; } = false ;  
        public DateTime CreatedOn { get; set; } = DateTime.Now;  
        public DateTime? UpdatedOn { get; set; }
    }
}
