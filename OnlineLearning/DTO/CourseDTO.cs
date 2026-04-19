using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;

namespace OnlineLearning.Models
{
    public class CourseDTO
    {
        public int CourseId { get; set; } 
        public string CourseTitle { get; set; } 
        public decimal Price { get; set; } 
        public int CategoryId { get; set; } 
        public int Level { get; set; } 
        public string Language { get; set; } 
        public int InstructorId { get; set; }
        public string? Thumbnail { get; set; } 
        public string Description { get; set; } 
        public bool IsPublished { get; set; }
        public bool IsActive { get; set; } 
        public string  CategoryName { get; set; }  
        public string? TotalLectures { get; set; }  
        public string? Duration { get; set; }  
        public string  LevelName { get; set; }  
        public bool IsDemo { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
       
    } 
     
}
