using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineLearning.Models
{
    public class PlanCourseVM
    {
        public int PlanId { get; set; }
        public List<int> SelectedCourseIds { get; set; } = new List<int>();
        public List<SelectListItem> Plans { get; set; }
        public List<SelectListItem> Courses { get; set; }
    }
}
