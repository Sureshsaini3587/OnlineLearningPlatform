using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ICourseVideoRepository: ICommonRepository<CourseVideoDTO>
    {
       Task<List<CourseVideoDTO>> GetAllWithDetails();
    }
}
