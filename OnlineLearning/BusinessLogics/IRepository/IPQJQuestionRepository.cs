using OnlineLearning.DTO;
using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IPQJQuestionRepository
    {
        Task<IEnumerable<PQJQuestion>> GetAllAsync();
        Task<PQJQuestion> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id, int userId);
        Task<int> CreateAsync(PQJQuestion q, List<PQJOption> options, int correctOption);
        Task<bool> UpdateAsync(PQJQuestion q, List<PQJOption> options, int correctOption);
Task<IEnumerable<CommanDTO>> GetCourse();
        Task<IEnumerable<CommanDTO>> GetCategory(); 
        
    }
}
