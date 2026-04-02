using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class SubscriptionPlan
    {
        public int PlanId { get; set; }

        [Required(ErrorMessage ="Plan name required")]
        public string PlanName { get; set; } 
        public decimal Price { get; set; } 
        public int DurationInDays { get; set; }
        public string Description { get; set; } 
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
}
