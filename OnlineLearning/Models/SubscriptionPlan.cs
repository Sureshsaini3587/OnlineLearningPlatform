using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class SubscriptionPlan
    {
        public int PlanId { get; set; }

        [Required(ErrorMessage = "Plan name is required")]
        [StringLength(100, ErrorMessage = "Plan name can't exceed 100 characters")]
        public string PlanName { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 3650, ErrorMessage = "Duration must be between 1 and 3650 days")]
        public int DurationInDays { get; set; }

        [StringLength(500, ErrorMessage = "Description can't exceed 500 characters")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
}
