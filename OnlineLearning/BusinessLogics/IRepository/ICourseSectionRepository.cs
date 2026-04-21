using Microsoft.AspNetCore.Razor.Language.Intermediate;
using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ICourseSectionRepository: ICommonRepository<CourseSectionDTO>
    {
       Task<List<CourseSectionDTO>> GetAllWithDetails();
       Task<List<CourseSectionDTO>> GetByCourseId(int courseid);
    }
}
