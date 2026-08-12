using Microsoft.AspNetCore.Razor.Language.Intermediate;
using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ICourseSectionRepository: ICommonRepository<CourseSectionDTO>
    {
       Task<List<CourseSectionDTO>> GetAllWithDetails();
       Task<List<CourseSectionDTO>> GetByCourseId(int courseid);
    }
    public interface ICourseSubSectionRepository
    {
        Task<IEnumerable<SubSection>> GetAllAsync(int? sectionId = null);
        Task<SubSection> GetByIdAsync(int subSectionId);
        Task<int> UpsertAsync(SubSection subSection);
        Task<bool> DeleteAsync(int subSectionId);
    }
}
