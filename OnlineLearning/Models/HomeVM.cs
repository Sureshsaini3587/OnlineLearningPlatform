using OnlineLearning.DTO;

namespace OnlineLearning.Models
{
    public class HomeVM
    {
        public List<CourseCategoriesDTO> Categories { get; set; }
        public List<CourseDTO> Courses { get; set; }
        public List<SubscriptionPlanDTO> Plans { get; set; }
    }
}
