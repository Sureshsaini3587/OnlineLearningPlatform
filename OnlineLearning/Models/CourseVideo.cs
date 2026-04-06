using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;

namespace OnlineLearning.Models
{

    public class CourseVideo
    {
        public int VideoId { get; set; }
        [Required(ErrorMessage = "Section is required")]
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title can't exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Video URL is required")]
        [Url(ErrorMessage = "Invalid video URL")]
        [StringLength(500)]
        public string VideoUrl { get; set; }

        [Range(0, 86400, ErrorMessage = "Duration must be between 0 and 86400 seconds")]
        public int Duration { get; set; } = 0;
        public bool IsDemo { get; set; }
        public int SortOrder { get; set; } 
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } 
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } 
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
