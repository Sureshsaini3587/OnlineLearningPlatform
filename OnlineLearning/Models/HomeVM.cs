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
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int TopicId { get; set; }
        public int Difficulty { get; set; }
        public List<CourseDTO> Courses { get; set; }
        public QuestionVM Question { get; set; } 
        public int? AttemptId { get; set; }
        public int CurrentIndex { get; set; }
        public int TotalQuestions { get; set; } 
        public int? SelectedOptionId { get; set; } 
        public string TimeLeft { get; set; } = "10:00";
    }
    public class PqjCourseQuizVM
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int TopicId { get; set; }
        public int Difficulty { get; set; }
        public List<PqjQuestionVM> Questions { get; set; } = new();
        public PqjQuestionVM Question { get; set; } = new();
    }
    public class PqjSingleQuestionVM
    {
        public int QuestionId { get; set; }
        public int AttemptId { get; set; }
        public int CurrentIndex { get; set; }
        public int TotalQuestions { get; set; }
        public string LectureTitle { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
    }

    public class PqjQuestionVM
    {
        public int QuestionId { get; set; }
        public string LectureTitle { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectOption { get; set; } = string.Empty; 
        public string Explanation { get; set; } = string.Empty;
    }
}
