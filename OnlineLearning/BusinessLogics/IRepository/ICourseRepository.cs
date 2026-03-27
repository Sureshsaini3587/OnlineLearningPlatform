using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ICourseRepository: ICommonRepository<Course>
    {
       Task<List<Course>> GetAllWithDetails();
    }
}
