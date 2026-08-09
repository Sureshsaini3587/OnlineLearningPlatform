using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IReadingQuestionRepository : ICommonRepository<ReadingQuestionVM>
    {     
         
        Task SaveOptionsAsync(int questionId, List<ReadingOptionVM> options);
    }
}
