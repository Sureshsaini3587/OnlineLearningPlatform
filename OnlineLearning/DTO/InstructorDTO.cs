namespace OnlineLearning.DTO
{
    public class InstructorDTO
    {
        public int? InstructorId { get; set; } 
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Role { get; set; }  
        public string? Bio { get; set; }
        public int? ExperienceYears { get; set; }
        public byte[]? ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
   
}
