namespace OnlineLearning.DTO
{
    public class SaveAnswerDTO
    {
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }
    }
    public class SaveAnswerResultDTO
    {
        public bool IsCorrect { get; set; } 
        public int CorrectOptionId { get; set; }
    }
}
