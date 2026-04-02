namespace OnlineLearning.DTO
{
    public class StudentDTO
    {
        public int? StudentId { get; set; }
        public int? UserID { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Role { get; set; }
        public DateTime? DOB { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public byte[]? ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
    public class GenderDTO
    {
        public int GenderId { get; set; }
        public string GenderName { get; set; }
    }
}
