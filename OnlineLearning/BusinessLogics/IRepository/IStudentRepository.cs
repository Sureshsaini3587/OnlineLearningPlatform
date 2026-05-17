using OnlineLearning.DTO;
using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IStudentRepository : ICommonRepository<StudentDTO>
    {
        Task<List<CourseDTO>> GetCourse();
        Task<StudentDashboardVM> GetDashboard(int userId);
       Task<List<StudentDTO>> GetAllWithDetails();
        Task<List<CourseDTO>> GetStudentCourses(int studentId);
       Task<List<GenderDTO>> GetGender();
    }
}
