using OnlineLearning.DTO;

namespace OnlineLearning.Models
{
    public class HomeVM
    {
        public List<CourseCategoriesDTO> Categories { get; set; }
        public List<CourseDTO> Courses { get; set; }
        public List<SubscriptionPlanDTO> Plans { get; set; }
    }
    
    
   
    public class PQJPlayerVM
    {
        public List<CourseDTO> Courses { get; set; }
        public QuestionVM Question { get; set; } 
        public int CurrentIndex { get; set; }
        public int TotalQuestions { get; set; } 
        public int? SelectedOptionId { get; set; } 
        public string TimeLeft { get; set; } = "10:00";
    }
}
