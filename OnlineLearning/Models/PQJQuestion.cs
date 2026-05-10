using OnlineLearning.Helpers.enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class QuestionVM
    {
        public PQJQuestion Question { get; set; }
        public List<PQJOption> Options { get; set; }
        public int CorrectOption { get; set; }
    }
    public class PQJQuestion
    {
        public int? QuestionId { get; set; }

        [Required(ErrorMessage = "Question text is required")]
        [StringLength(2000)]
        public string QuestionText { get; set; }
         

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Course is required")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Question type is required")]
        public QuestionType QuestionType { get; set; }

        [Range(1, 3, ErrorMessage = "Invalid difficulty level")]
        public int DifficultyLevel { get; set; } 

        [Range(0, 1000)]
        public int Marks { get; set; } = 1; 

        public bool IsTrial { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        [Required]
        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
         
        public virtual List<PQJOption> Options { get; set; } = new List<PQJOption>();

        public string? Explanation { get; set; }
         
    }
    public class PQJOption
    {
        public int? OptionId { get; set; }
         
        public int? QuestionId { get; set; }

        [Required(ErrorMessage = "Option text is required")]
        [StringLength(500)]
        public string OptionText { get; set; } 
        public bool IsCorrect { get; set; }   
    }
    public class PQJExplanation
    {
        public int ExplanationId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [StringLength(2000)]
        public string ExplanationText { get; set; } 
         
    }
    public class PQJAttemptAnswer
    {
        public int Id { get; set; }

        [Required]
        public int AttemptId { get; set; }

        [Required]
        public int QuestionId { get; set; }
         
        public int? SelectedOptionId { get; set; }
         
        [StringLength(2000)]
        public string? AnswerText { get; set; }
         
        public bool? IsCorrect { get; set; } 
           
        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        [Required]
        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; } 
    }
}
