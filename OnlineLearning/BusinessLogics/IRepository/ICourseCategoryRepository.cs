using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ICourseCategoryRepository : ICommonRepository<CourseCategoriesDTO>
    {
       Task<List<CourseCategoriesDTO>> GetAllWithDetails();
    }
}
