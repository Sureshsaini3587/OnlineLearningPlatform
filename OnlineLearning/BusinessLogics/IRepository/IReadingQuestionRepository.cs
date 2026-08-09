using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IReadingQuestionRepository : ICommonRepository<ReadingQuestionVM>
    {
        Task<StudentReadingVM> GetStudentReadingSyllabusAsync(int userId);
        Task<IEnumerable<ReadingQuestionVM>> GetQuestionsBySectionAsync(int courseId, int sectionId);
        Task<IEnumerable<ReadingOptionVM>> GetOptionsByQuestionIdAsync(int questionId);
        Task SaveOptionsAsync(int questionId, List<ReadingOptionVM> options);
    }
}
