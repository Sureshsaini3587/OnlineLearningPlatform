using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ICourseRepository: ICommonRepository<CourseDTO>
    {
       Task<List<CourseDTO>> GetAllWithDetails();
    }
}
