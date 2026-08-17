using OnlineLearning.DTO;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IPQJQuestionRepository
    {
        Task<IEnumerable<PQJQuestion>> GetAllAsync();
        Task<PQJQuestion> GetByIdAsync(int id);
        Task<List<QuestionVM>> GetFilteredQuestions(int? courseId, int? topicId, int difficulty,string Mode);
        Task<int> GetOrCreateAttempt(int courseId);
        Task CompleteAttempt(int attemptId);
        Task<Result> DeleteAsync(int id, int userId);
        Task<Result> CreateAsync(PQJQuestion q, List<PQJOption> options, int correctOption);
        Task<Result> UpdateAsync(PQJQuestion q, List<PQJOption> options, int correctOption);
         Task<IEnumerable<CommanDTO>> GetCourse();
        Task<IEnumerable<CommanDTO>> GetCategory(); 
        Task<SaveAnswerResultDTO> SaveAnswer(SaveAnswerDTO dto);

    }
}
