namespace OnlineLearning.Models
{
    public class StudentReadingVM
    {
        public int QuestionId { get; set; }
        public int QuestionNumber { get; set; }
        public string QuestionTextEn { get; set; }
        public string QuestionTextHi { get; set; }
        public int DifficultyLevel { get; set; }
        public string? DiagramImageUrl { get; set; }
        public string? ExplanationText { get; set; }

        public int ExamId { get; set; }
        public int Year { get; set; }
        public string Shift { get; set; }

        public List<StudentOptionVM> Options { get; set; } = new List<StudentOptionVM>();
         
        public int TotalQuestions { get; set; }
        public int AttemptedCount { get; set; }
        public int CurrentIndex { get; set; }
    }

    public class StudentOptionVM
    {
        public int OptionId { get; set; }
        public string OptionTextEn { get; set; }
        public string OptionTextHi { get; set; }
        public bool IsCorrect { get; set; }
    }
}
