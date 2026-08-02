using OnlineLearning.Models.ResponseModel;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ICommonRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<Result> AddAsync(T entity);
        Task<Result> UpdateAsync(T entity);
        Task<Result> DeleteAsync(T entity);
    }
}
