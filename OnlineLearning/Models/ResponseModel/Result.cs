namespace OnlineLearning.Models.ResponseModel
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? NewId { get; set; }
    }
}
