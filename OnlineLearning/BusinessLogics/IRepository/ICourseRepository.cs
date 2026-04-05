using OnlineLearning.DTO;
using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ICourseRepository: ICommonRepository<CourseDTO>
    {
       Task<List<CourseDTO>> GetAllWithDetails();
       Task<List<CommanDTO>> GetLanguage();
       Task<List<CommanDTO>> GetInstructor();
    }
}
