namespace OnlineLearning.Models
{
    public class Subscription
    {
        public int? SubscriptionId { get; set; } 
        public int StudentId { get; set; }
        public int PlanId { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 
        public string Status { get; set; }  
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } 
        public int? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } 
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

    }
}
