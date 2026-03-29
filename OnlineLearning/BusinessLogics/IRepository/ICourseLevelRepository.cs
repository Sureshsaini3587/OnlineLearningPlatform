using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ICourseLevelRepository : ICommonRepository<CourseLevelDTO>
    {
       Task<List<CourseLevelDTO>> GetAllWithDetails();
    }
}
