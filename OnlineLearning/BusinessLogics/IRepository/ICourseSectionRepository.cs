using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ICourseSectionRepository: ICommonRepository<CourseSectionDTO>
    {
       Task<List<CourseSectionDTO>> GetAllWithDetails();
    }
}
