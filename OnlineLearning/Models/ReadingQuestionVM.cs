namespace OnlineLearning.Models
{
    public class ReadingQuestionVM
    {
        public int QuestionId { get; set; } 
        public string QuestionTextEn { get; set; }  
        public string QuestionTextHi { get; set; } 

        public int CourseId { get; set; }
        public int SectionId { get; set; }              
        public int Year { get; set; }        
        public string Shift { get; set; }    
        public int QuestionNumber { get; set; } 
        public int DifficultyLevel { get; set; }  

        public string? DiagramImageUrl { get; set; }  
        public string? ExplanationText { get; set; }

        public bool IsActive { get; set; } = true; 
        public List<ReadingOptionVM> Options { get; set; } = new List<ReadingOptionVM>();
    }

    public class ReadingOptionVM
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public string OptionTextEn { get; set; }
        public string OptionTextHi { get; set; }
        public bool IsCorrect { get; set; }
    }
}
